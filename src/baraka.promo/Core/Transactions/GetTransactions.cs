using baraka.promo.Models.LoyaltyApiModels.Cardholders;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Models;
using MediatR;
using baraka.promo.Models.LoyaltyApiModels.Transactions;
using baraka.promo.Core.Cardholders;
using baraka.promo.Data;
using baraka.promo.Models.LoyaltyApiModels.Cards;
using baraka.promo.Services;
using baraka.promo.Utils;
using Microsoft.Extensions.Caching.Memory;

namespace baraka.promo.Core.Transactions
{
    public class GetTransactions
    {
        public class Command : IRequest<ApiBaseResultModel<ListBaseModel<TransactionInfoModel>>>
        {
            public Command(TransactionRequestModel model)
            {
                Model = model;
            }

            public TransactionRequestModel Model { get; private set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<ListBaseModel<TransactionInfoModel>>>
        {
            readonly ILogger<GetTransactions> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<GetTransactions> logger, ApplicationDbContext db, ICurrentUser currentUser, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<ListBaseModel<TransactionInfoModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null) return new ApiBaseResultModel<ListBaseModel<TransactionInfoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));

                    var model = request.Model;

                    var cache_key = TransactionInfoModel.CACHE_KEY + model.CardId + model.CardNumber;
                    if (!_memory_cache.TryGetValue(cache_key, out List<TransactionInfoModel> result))
                    {
                        result = (from x in _db.Transactions
                                  where x.CardId == model.CardId || x.CardNumber == model.CardNumber
                                  select new TransactionInfoModel
                                  {
                                      Id = x.Id,
                                      CardId = x.CardId,
                                      CardNumber = x.CardNumber,
                                      Status = x.Status,
                                      Sum = x.Sum,
                                      Type = x.Type,
                                      CreatedBy = x.CreatedBy,
                                      CreatedTime = x.CreatedTime,
                                      CanceledTime = x.CanceledTime,

                                  }).OrderByDescending(x => x.CreatedTime).ToList();

                        _memory_cache.Set(cache_key, result, DateTime.Now.AddSeconds(5));
                    }

                    List<TransactionInfoModel> filter_query = new List<TransactionInfoModel>();

                    if (request.Model.Take > 0)
                    {
                        if (request.Model.Skip > 0) filter_query = result.Skip(request.Model.Skip).Take(request.Model.Take).ToList();
                        else filter_query = result.Take(request.Model.Take).ToList();
                    }
                    else filter_query = result;

                    return new ApiBaseResultModel<ListBaseModel<TransactionInfoModel>>(new ListBaseModel<TransactionInfoModel> { List = filter_query, Total = result.Count });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<ListBaseModel<TransactionInfoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
