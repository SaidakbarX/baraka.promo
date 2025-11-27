using baraka.promo.Data;
using baraka.promo.Models;
using baraka.promo.Models.LoyaltyApiModels.Transactions;
using baraka.promo.Utils;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace baraka.promo.Core.Transactions
{
    public class GetBonusTransactions
    {
        public class Command : IRequest<ApiBaseResultModel<ListBaseModel<TransactionBonusInfoModel>>>
        {
            public Command(TransactionRequestModel model)
            {
                Model = model;
            }

            public TransactionRequestModel Model { get; private set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<ListBaseModel<TransactionBonusInfoModel>>>
        {
            readonly ILogger<GetBonusTransactions> _logger;
            readonly ApplicationDbContext _db;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<GetBonusTransactions> logger, ApplicationDbContext db, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<ListBaseModel<TransactionBonusInfoModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var model = request.Model;

                    var cache_key = TransactionBonusInfoModel.CACHE_KEY + model.CardId;
                    if (!_memory_cache.TryGetValue(cache_key, out List<TransactionBonusInfoModel> result))
                    {
                        result = (from x in _db.Transactions
                                  join b in _db.BonusHistories on x.Id equals b.TransactionId into cs
                                  from b in cs.DefaultIfEmpty()
                                  where x.CardId == model.CardId && x.Type == Models.Enums.TransactionType.WriteOff
                                  select new TransactionBonusInfoModel
                                  {
                                      Id = x.Id,
                                      CardId = x.CardId,
                                      Status = x.Status.ToString(),
                                      Sum = x.Sum,
                                      CreatedTime = x.CreatedTime,
                                      CanceledTime = x.CanceledTime,
                                      CashbackSum = b != null ? b.CashbackSum : 0,
                                  }).OrderByDescending(x => x.CreatedTime).ToList();

                        _memory_cache.Set(cache_key, result, DateTime.Now.AddSeconds(5));
                    }

                    List<TransactionBonusInfoModel> filter_query = new List<TransactionBonusInfoModel>();

                    if (request.Model.Take > 0)
                    {
                        if (request.Model.Skip > 0) filter_query = result.Skip(request.Model.Skip).Take(request.Model.Take).ToList();
                        else filter_query = result.Take(request.Model.Take).ToList();
                    }
                    else filter_query = result;

                    return new ApiBaseResultModel<ListBaseModel<TransactionBonusInfoModel>>(new ListBaseModel<TransactionBonusInfoModel> { List = filter_query, Total = result.Count });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<ListBaseModel<TransactionBonusInfoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
