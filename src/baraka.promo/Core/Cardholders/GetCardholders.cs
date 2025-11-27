using baraka.promo.Models.LoyaltyApiModels.Cardholders;
using baraka.promo.Models;
using MediatR;
using baraka.promo.Data;
using baraka.promo.Models.LoyaltyApiModels.Cards;
using baraka.promo.Services;
using baraka.promo.Utils;
using Microsoft.Extensions.Caching.Memory;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;

namespace baraka.promo.Core.Cardholders
{
    public class GetCardholders
    {
        public class Command : IRequest<ApiBaseResultModel<ListBaseModel<CardholderInfoModel>>>
        {
            public Command(FilterModel filter)
            {
                Filter = filter;
            }

            public FilterModel Filter { get; private set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<ListBaseModel<CardholderInfoModel>>>
        {
            readonly ILogger<GetCardholders> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<GetCardholders> logger, ApplicationDbContext db, ICurrentUser currentUser, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<ListBaseModel<CardholderInfoModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null) return new ApiBaseResultModel<ListBaseModel<CardholderInfoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));
                    //if (!_current_user.IsAdmin()) return new ApiBaseResultModel<ListBaseModel<CardholderInfoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));

                    var cache_key = CardholderInfoModel.CACHE_KEY;
                    if (!_memory_cache.TryGetValue(cache_key, out List<CardholderInfoModel> result))
                    {
                        var cardholders = _db.Cardholders.Where(x => !x.IsDeleted).ToList();

                        var cards = (from x in _db.Cards
                                     where !x.IsDeleted
                                     select new CardInfoModel
                                     {
                                         Id = x.Id,
                                         Number = x.Number,
                                         UserId = x.UserId,
                                         Balance = x.Balance,
                                         Type = x.Type,
                                     }).ToList();

                        result = (from x in cardholders
                                  select new CardholderInfoModel
                                  {
                                      Id = x.Id,
                                      Name = x.Name,
                                      Phone = x.Phone,
                                      Type = x.Type,
                                      CreatedTime = x.CreatedTime,
                                      ModifiedTime = x.ModifiedTime,
                                      DateOfBirth = x.DateOfBirth.HasValue ? x.DateOfBirth.Value.ToShortDateString() : null,
                                      Cards = cards.Where(z => z.UserId == x.Id).ToList(),

                                  }).OrderByDescending(x => x.ModifiedTime).ToList();

                        _memory_cache.Set(cache_key, result, DateTime.Now.AddSeconds(3));
                    }

                    List<CardholderInfoModel> filter_query = new List<CardholderInfoModel>();

                    if (!string.IsNullOrEmpty(request.Filter.SearchText)) result = result.Where(x => x.Phone.Contains(request.Filter.SearchText)
                                                                                                  || x.Name.Contains(request.Filter.SearchText)
                                                                                                  || x.CardNumber.Contains(request.Filter.SearchText)).ToList();

                    if (request.Filter.Take > 0)
                    {
                        if (request.Filter.Skip > 0) filter_query = result.Skip(request.Filter.Skip).Take(request.Filter.Take).ToList();
                        else filter_query = result.Take(request.Filter.Take).ToList();
                    }
                    else filter_query = result;
                   
                    return new ApiBaseResultModel<ListBaseModel<CardholderInfoModel>>(new ListBaseModel<CardholderInfoModel> { List = filter_query, Total = result.Count });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<ListBaseModel<CardholderInfoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
