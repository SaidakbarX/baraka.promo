using baraka.promo.Models;
using MediatR;
using baraka.promo.Models.TgMessageModel;
using baraka.promo.Data;
using baraka.promo.Services;
using baraka.promo.Utils;
using baraka.promo.Pages.TgPushSender;
using baraka.promo.BackgroundService.BackgroundModels;
using System.Transactions;
using Newtonsoft.Json;

namespace baraka.promo.Core.Messages
{
    public class MessageSender
    {
        public class Command : IRequest<ApiBaseResultModel<bool>>
        {
            public Command(MessageModel model, string integration_name)
            {
                Model = model ?? throw new ArgumentNullException(nameof(model));
                IntegrationName = integration_name;
            }

            public MessageModel Model { get; set; }
            public string IntegrationName { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<bool>>
        {
            readonly ILogger<MessageSender> _logger;
            readonly ApplicationDbContext _db;
            readonly TgHelper _tg_helper;
            readonly TasksToRun _tasks_to_run;

            public Handler(ILogger<MessageSender> logger, ApplicationDbContext db,
                TgHelper tgHelper, TasksToRun tasks_to_run)
            {
                _logger = logger;
                _db = db;
                _tg_helper = tgHelper;
                _tasks_to_run = tasks_to_run;
            }

            public async Task<ApiBaseResultModel<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    //var user = _current_user.GetCurrentUserName();
                    //if (user == null) return new ApiBaseResultModel<bool>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));
                    //if (!_current_user.IsAdmin()) return new ApiBaseResultModel<bool>(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));

                    var model = request.Model;

                    _logger.LogWarning($"MessageSender -> {JsonConvert.SerializeObject(model)}");

                    var clients = await _tg_helper.GetClientsTgIds(model.Phones, true, true, true);

                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        try
                        {
                            var guid = Guid.NewGuid();

                            await _db.MessageHeaders.AddAsync(new MessageHeader
                            {
                                Id = guid,
                                IsImage = false,
                                Message = model.Message,
                                WorkingTimeFrom = model.TimeFrom,
                                WorkingTimeTo = model.TimeTo,
                                Status = MessageHeaderStatus.Start,
                                CreatedTime = DateTime.Now,
                                CreatedBy = request.IntegrationName,
                                StartDate = model.StartTime >= DateTime.Today ? model.StartTime : DateTime.Today,
                                JsonButtons = model.ButtonInfo != null && model.ButtonInfo.Count > 0 ? 
                                JsonConvert.SerializeObject(model.ButtonInfo.Select(x=> new { Name = x.Text, x.Url })) : null,
                            });
                            await _db.SaveChangesAsync();

                            _tasks_to_run.Enqueue(TaskSettings.FromMonitorSettings(new BackgroundService.SettingsModel
                            {
                                ChatIds = clients,
                                MessageHeaderId = guid
                            }));

                            scope.Complete();
                        }
                        catch (Exception ex)
                        {
                            scope.Dispose();
                            _logger.LogError(ex, ex.Message);
                            return new ApiBaseResultModel<bool>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                        }
                    }

                    return new ApiBaseResultModel<bool>();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<bool>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
