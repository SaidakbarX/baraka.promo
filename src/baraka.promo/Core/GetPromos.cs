using baraka.promo.Data;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace baraka.promo.Core
{
    public class GetPromos
    {
        public class Command : IRequest<ApiBaseResultModel<ListBaseModel<PromoModel>>>
        {
            public Command(PageFilterModel filter)
            {
                Filter = filter;
            }

            public PageFilterModel Filter { get; private set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<ListBaseModel<PromoModel>>>
        {
            readonly ILogger<GetPromos> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<GetPromos> logger, ApplicationDbContext db, ICurrentUser currentUser, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
                _memory_cache = memoryCache;
            }
            
            public async Task<ApiBaseResultModel<ListBaseModel<PromoModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    //var user = _current_user.GetCurrentUserName();
                    //if (user == null) return new ApiBaseResultModel<ListBaseModel<PromoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));

                    var cache_key = PromoModel.CACHE_KEY;
                    if (!_memory_cache.TryGetValue(cache_key, out List<PromoModel> result))
                    {
                        result = (from x in _db.Promos
                                  where !x.IsDeleted && x.IsActive && !x.IsUnique &&
                                  x.StartTime < DateTime.Now && (!x.EndTime.HasValue || x.EndTime > DateTime.Now)
                                  select new PromoModel
                                  {
                                      Id = x.Id,
                                      Name = x.Name,
                                      View = x.View,
                                      Type = x.Type,
                                      StartTime = x.StartTime,
                                      EndTime = x.EndTime,
                                  }).ToList();

                        _memory_cache.Set(cache_key, result, DateTime.Now.AddSeconds(15));
                    }

                    List<PromoModel> filter_query = new List<PromoModel>();

                    if (request.Filter.Take > 0)
                    {
                        if (request.Filter.Skip > 0) filter_query = result.Skip(request.Filter.Skip).Take(request.Filter.Take).ToList();
                        else filter_query = result.Take(request.Filter.Take).ToList();
                    }
                    else filter_query = result;

                    return new ApiBaseResultModel<ListBaseModel<PromoModel>>(new ListBaseModel<PromoModel> { List = filter_query, Total = result.Count });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<ListBaseModel<PromoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
