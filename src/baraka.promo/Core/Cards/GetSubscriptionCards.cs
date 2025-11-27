using baraka.promo.Data;
using baraka.promo.Models.LoyaltyApiModels.LoyalityTypeModels;
using baraka.promo.Models;
using baraka.promo.Utils;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using baraka.promo.Models.Enums;

namespace baraka.promo.Core.Cards
{
    public class GetSubscriptionCards
    {
        public class Command : IRequest<ApiBaseResultModel<List<SubscriptionModel>>>
        {
            public Command()
            {

            }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<List<SubscriptionModel>>>
        {
            readonly ILogger<GetSubscriptionCards> _logger;
            readonly ApplicationDbContext _db;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<GetSubscriptionCards> logger, ApplicationDbContext db, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<List<SubscriptionModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var cache_key = TextConstants.SubscriptionsInfoCacheKey;

                    if (!_memory_cache.TryGetValue(cache_key, out List<SubscriptionModel> result))
                    {
                        var card_subs = await _db.LoyalityTypes.FirstOrDefaultAsync(x => x.Type == LoyalityTypeKey.CARD_SUBSCRIPTION.ToString());

                        if (card_subs != null)
                        {
                            result = JsonConvert.DeserializeObject<List<SubscriptionModel>>(card_subs.ValueInfo);
                        }
                        else result = new List<SubscriptionModel>();

                        _memory_cache.Set(cache_key, result, DateTimeOffset.Now.AddMinutes(1));
                    }

                    return new ApiBaseResultModel<List<SubscriptionModel>>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<List<SubscriptionModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
