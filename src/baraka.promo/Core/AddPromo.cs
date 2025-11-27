using baraka.promo.Data;
using baraka.promo.Models;
using baraka.promo.Models.PromoApi;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace baraka.promo.Core
{
    public class AddPromo
    {
        public class Command : IRequest<ApiBaseResultModel<PromoResultModel>>
        {
            public Command(PromoApiModel model)
            {
                Model = model;
            }

            public PromoApiModel Model { get; private set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<PromoResultModel>>
        {
            readonly ILogger<AddPromo> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;
            readonly IMemoryCache _memory_cache;
            readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            public Handler(ILogger<AddPromo> logger, ApplicationDbContext db, ICurrentUser currentUser, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<PromoResultModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null) return new ApiBaseResultModel<PromoResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));

                    var model = request.Model;
                    _logger.LogWarning($"AddPromo -> {JsonConvert.SerializeObject(model)}");

                    Random random = new Random();

                    char[] result = new char[5];
                    for (int i = 0; i < 5; i++)
                    {
                        result[i] = chars[random.Next(chars.Length)];
                    }

                    Promo promo = new Promo
                    {
                        Name = new string(result),
                        MaxCount = model.MaxCountForClient,
                        StartTime = model.StartTime,
                        EndTime = model.EndTime,
                        MinOrderAmount = model.MinOrderAmount.HasValue ? (long)Math.Ceiling(model.MinOrderAmount.Value * 100) : null,
                        MaxOrderAmount = model.MaxOrderAmount.HasValue ? (long)Math.Ceiling(model.MaxOrderAmount.Value * 100) : null,
                        OrderDiscount = model.OrderDiscount,
                        Type = PromoType.Personal,
                        View = (PromoView)model.View,
                        TotalCount = model.TotalCount,
                        //IsActive = true,
                    };

                    await _db.Promos.AddAsync(promo, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);

                    await _db.PromoClients.AddAsync(new PromoClient
                    {
                        Phone = model.Phone.Replace(" ", ""),
                        PromoId = promo.Id,
                    });

                    List<PromoRegion> promo_regions = new List<PromoRegion>();
                    List<PromoRestaurant> promo_restaurants = new List<PromoRestaurant>();
                    List<PromoArbitration> promo_arbitrations = new List<PromoArbitration>();
                    List<PromoProduct> promo_products = new List<PromoProduct>();
                    List<PromoProductFeature> promo_product_features = new List<PromoProductFeature>();

                    if (model.Regions?.Count > 0)
                    {
                        foreach (var id in model.Regions)
                        {
                            promo_regions.Add(new PromoRegion
                            {
                                PromoId = promo.Id,
                                RegionId = id
                            });
                        }
                    }

                    if (model.Restaurants?.Count > 0)
                    {
                        foreach (var id in model.Restaurants)
                        {
                            promo_restaurants.Add(new PromoRestaurant
                            {
                                PromoId = promo.Id,
                                RestaurantId = id
                            });
                        }
                    }

                    if (model.ArbitrationProducts?.Count > 0)
                    {
                        foreach (var id in model.ArbitrationProducts)
                        {
                            promo_arbitrations.Add(new PromoArbitration
                            {
                                PromoId = promo.Id,
                                ProductId = id,
                            });
                        }
                    }                  

                    if (promo.View == PromoView.FreeProduct)
                    {
                        if (model.DiscountProducts?.Count > 0)
                        {
                            foreach (var item in model.DiscountProducts)
                            {
                                promo_products.Add(new PromoProduct
                                {
                                    ProductId = item.Id,
                                    Count = item.Count,
                                    PromoId = promo.Id,
                                });
                            }
                        }
                    }

                    else if (promo.View == PromoView.ProductDiscount)
                    {
                        if (model.DiscountProducts?.Count > 0)
                        {
                            foreach (var item in model.DiscountProducts)
                            {
                                promo_products.Add(new PromoProduct
                                {
                                    ProductId = item.Id,
                                    Count = item.Count,
                                    PromoId = promo.Id,
                                    Discount = item.Discount,
                                });
                            }
                        }
                    }

                    else if (promo.View == PromoView.OrderMinSumPromotion)
                    {
                        if (model.DiscountProducts?.Count > 0)
                        {
                            foreach (var item in model.DiscountProducts)
                            {
                                promo_products.Add(new PromoProduct
                                {
                                    ProductId = item.Id,
                                    Count = item.Count,
                                    PromoId = promo.Id,
                                });
                            }
                        }
                    }

                    else if (promo.View == PromoView.PromotionalProduct)
                    {
                        if (model.DiscountProducts?.Count > 0)
                        {
                            foreach (var item in model.DiscountProducts)
                            {
                                promo_products.Add(new PromoProduct
                                {
                                    ProductId = item.Id,
                                    Count = item.Count,
                                    PromoId = promo.Id,
                                });
                            }
                        }
                        if (model.PromotionalProducts?.Count > 0)
                        {
                            foreach (var item in model.PromotionalProducts)
                            {
                                promo_product_features.Add(new PromoProductFeature
                                {
                                    ProductId = item.Id,
                                    Count = item.Count,
                                    PromoId = promo.Id,
                                });
                            }
                        }
                    }

                    if (promo_regions.Count > 0) await _db.PromoRegions.AddRangeAsync(promo_regions);
                    if (promo_restaurants.Count > 0) await _db.PromoRestaurants.AddRangeAsync(promo_restaurants);
                    if (promo_arbitrations.Count > 0) await _db.PromoArbitrations.AddRangeAsync(promo_arbitrations);
                    if (promo_products.Count > 0) await _db.PromoProducts.AddRangeAsync(promo_products);
                    if (promo_product_features.Count > 0) await _db.PromoProductFeatures.AddRangeAsync(promo_product_features);

                    await _db.SaveChangesAsync(cancellationToken);

                    var result_model = new ApiBaseResultModel<PromoResultModel>(new PromoResultModel { Id = promo.Id, Code = promo.Name });
                    _logger.LogWarning($"AddPromo result -> {JsonConvert.SerializeObject(result_model)}");

                    return result_model;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<PromoResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
