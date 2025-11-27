using baraka.promo.BackgroundService.BackgroundModels;
using baraka.promo.Data;
using baraka.promo.Delivery;
using baraka.promo.Models.Enums;
using baraka.promo.Models.SubscriptionsModels;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using Microsoft.Extensions.Options;
using baraka.promo.Models.ClickModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace baraka.promo.Core.Subscriptions
{
    public class CheckSubscription
    {
        public class Command : IRequest<ClickPrepareResultModel>
        {
            public Command(ClickPrepareModel model, int merchantId)
            {
                Model = model ?? throw new ArgumentNullException(nameof(model));
                MerchantId = merchantId;
            }

            public ClickPrepareModel Model { get; set; }
            public int MerchantId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ClickPrepareResultModel>
        {
            readonly ILogger<CheckSubscription> _logger;
            readonly ApplicationDbContext _db;
            readonly ClickSettingsModel _settings;

            public Handler(ILogger<CheckSubscription> logger, ApplicationDbContext db, IOptions<ClickSettingsModel> options)
            {
                _logger = logger;
                _db = db;
                _settings = options != null ? options.Value : new ClickSettingsModel();
            }

            public async Task<ClickPrepareResultModel> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    if(request.MerchantId != _settings.MerchantId) return new ClickPrepareResultModel { error = -5 };

                    var model = request.Model;
                    _logger.LogWarning($"CheckSubscription -> {JsonConvert.SerializeObject(model)}");

                    Guid transaction_id = Guid.Parse(model.merchant_trans_id);

                    ClickPrepareResultModel result;

                    if (await _db.Transactions.AnyAsync(x=>x.Id == transaction_id && x.Status == TransactionStatus.Draft))
                    {
                        result = new ClickPrepareResultModel
                        {
                            click_trans_id = model.click_trans_id,
                            merchant_trans_id = model.merchant_trans_id,
                            merchant_prepare_id = await _db.Transactions.CountAsync(),
                        };
                    }
                    else result = new ClickPrepareResultModel { error = -5 };

                    _logger.LogWarning($"CheckSubscription send -> {JsonConvert.SerializeObject(result)}");

                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ClickPrepareResultModel { error = -5 };
                }
            }
        }
    }
}
