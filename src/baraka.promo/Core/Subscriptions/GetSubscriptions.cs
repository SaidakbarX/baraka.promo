using baraka.promo.Core.Messages;
using baraka.promo.Data;
using baraka.promo.Models.PushModels;
using baraka.promo.Models;
using baraka.promo.Utils;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using baraka.promo.Models.SubscriptionsModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace baraka.promo.Core.Subscriptions
{
    public class GetSubscriptions
    {
        public class Command : IRequest<ApiBaseResultModel<SubscriptionApiModel>>
        {
            public Command(int countryId)
            {
                CountryId = countryId;
            }

            public int CountryId { get; private set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<SubscriptionApiModel>>
        {
            readonly ILogger<GetSubscriptions> _logger;
            readonly ApplicationDbContext _db;
            readonly IMemoryCache _memory_cache;
            readonly ProjectSettingsModel _settings;

            public Handler(ILogger<GetSubscriptions> logger, ApplicationDbContext db,
                IMemoryCache memoryCache, IOptions<ProjectSettingsModel> settings)
            {
                _logger = logger;
                _db = db;
                _memory_cache = memoryCache;
                _settings = settings != null ? settings.Value : new ProjectSettingsModel();
            }

            public async Task<ApiBaseResultModel<SubscriptionApiModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var cache_key = SubscriptionApiModel.CACHE_KEY + request.CountryId;

                    if (!_memory_cache.TryGetValue(cache_key, out SubscriptionApiModel result))
                    {
                        // 1. Avval subscriptions ni olib olamiz
                        var subscriptions = await (from x in _db.Subscriptions
                                                   where x.IsActive && x.CountryId == request.CountryId
                                                   select new SubscriptionModel
                                                   {
                                                       Id = x.Id,
                                                       MaxCount = x.MaxCount,
                                                       GroupId = x.GroupId,
                                                       ProductId = x.ProductId,
                                                       PromotionId = JsonConvert.DeserializeObject<List<string>>(x.PromotionId),
                                                       CountryId = x.CountryId,
                                                       Price = x.Price,
                                                       DisplayInfo = new DisplayModel
                                                       {
                                                           NameEn = x.NameEn,
                                                           NameRu = x.NameRu,
                                                           NameUz = x.NameUz,
                                                           ShortContent_En = x.ShortContent_En,
                                                           ShortContent_Ru = x.ShortContent_Ru,
                                                           ShortContent_Uz = x.ShortContent_Uz,
                                                           Content_En = x.Content_En,
                                                           Content_Ru = x.Content_Ru,
                                                           Content_Uz = x.Content_Uz,
                                                           ImageUrlUz = x.ImageUrlUz != null ? $"{_settings.HomeUrl}/api/image/{x.ImageUrlUz}" : null,
                                                           ImageUrlEn = x.ImageUrlEn != null ? $"{_settings.HomeUrl}/api/image/{x.ImageUrlEn}" : null,
                                                           ImageUrlRu = x.ImageUrlRu != null ? $"{_settings.HomeUrl}/api/image/{x.ImageUrlRu}" : null,
                                                       },
                                                       DisplayPurchaseInfo = new DisplayModel
                                                       {
                                                           NameEn = x.NamePurchaseEn,
                                                           NameRu = x.NamePurchaseRu,
                                                           NameUz = x.NamePurchaseUz,
                                                           ShortContent_En = x.ShortContent_Purchase_En,
                                                           ShortContent_Ru = x.ShortContent_Purchase_Ru,
                                                           ShortContent_Uz = x.ShortContent_Purchase_Uz,
                                                           Content_En = x.Content_Purchase_En,
                                                           Content_Ru = x.Content_Purchase_Ru,
                                                           Content_Uz = x.Content_Purchase_Uz,
                                                           ImageUrlUz = x.ImageUrl_Purchase_Uz != null ? $"{_settings.HomeUrl}/api/image/{x.ImageUrl_Purchase_Uz}" : null,
                                                           ImageUrlEn = x.ImageUrl_Purchase_En != null ? $"{_settings.HomeUrl}/api/image/{x.ImageUrl_Purchase_En}" : null,
                                                           ImageUrlRu = x.ImageUrl_Purchase_Ru != null ? $"{_settings.HomeUrl}/api/image/{x.ImageUrl_Purchase_Ru}" : null,
                                                       }
                                                   }).ToListAsync(cancellationToken);


                        var subscriptions_groups = await (from x in _db.SubscriptionGroups
                                                          where x.IsActive && x.CountryId == request.CountryId
                                                          select new SubscriptionGroupModel
                                                          {
                                                              Id = x.Id,
                                                              ValidityPeriod = x.ValidityPeriod,
                                                              CountryId = x.CountryId,
                                                              DisplayInfo = new DisplayModel
                                                              {
                                                                  NameEn = x.NameEn,
                                                                  NameRu = x.NameRu,
                                                                  NameUz = x.NameUz,
                                                                  ShortContent_En = x.ShortContent_En,
                                                                  ShortContent_Ru = x.ShortContent_Ru,
                                                                  ShortContent_Uz = x.ShortContent_Uz,
                                                                  Content_En = x.Content_En,
                                                                  Content_Ru = x.Content_Ru,
                                                                  Content_Uz = x.Content_Uz,
                                                                  ImageUrlUz = x.ImageUrlUz != null ? $"{_settings.HomeUrl}/api/image/{x.ImageUrlUz}" : null,
                                                                  ImageUrlEn = x.ImageUrlEn != null ? $"{_settings.HomeUrl}/api/image/{x.ImageUrlEn}" : null,
                                                                  ImageUrlRu = x.ImageUrlRu != null ? $"{_settings.HomeUrl}/api/image/{x.ImageUrlRu}" : null,
                                                              },
                                                              DisplayPurchaseInfo = new DisplayModel
                                                              {
                                                                  NameEn = x.NamePurchaseEn,
                                                                  NameRu = x.NamePurchaseRu,
                                                                  NameUz = x.NamePurchaseUz,
                                                                  ShortContent_En = x.ShortContent_Purchase_En,
                                                                  ShortContent_Ru = x.ShortContent_Purchase_Ru,
                                                                  ShortContent_Uz = x.ShortContent_Purchase_Uz,
                                                                  Content_En = x.Content_Purchase_En,
                                                                  Content_Ru = x.Content_Purchase_Ru,
                                                                  Content_Uz = x.Content_Purchase_Uz,
                                                                  ImageUrlUz = x.ImageUrl_Purchase_Uz != null ? $"{_settings.HomeUrl}/api/image/{x.ImageUrl_Purchase_Uz}" : null,
                                                                  ImageUrlEn = x.ImageUrl_Purchase_En != null ? $"{_settings.HomeUrl}/api/image/{x.ImageUrl_Purchase_En}" : null,
                                                                  ImageUrlRu = x.ImageUrl_Purchase_Ru != null ? $"{_settings.HomeUrl}/api/image/{x.ImageUrl_Purchase_Ru}" : null,
                                                              }
                                                          }).ToListAsync(cancellationToken);

                        result = new SubscriptionApiModel { groups = subscriptions_groups, subscriptions = subscriptions };

                        _memory_cache.Set(cache_key, result, DateTime.Now.AddSeconds(5));
                    }

                    return new ApiBaseResultModel<SubscriptionApiModel>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<SubscriptionApiModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
