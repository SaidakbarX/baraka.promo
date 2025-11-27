using System.Text;
using baraka.promo.Data;
using baraka.promo.Delivery;
using baraka.promo.Models;
using baraka.promo.Models.OrderApiModel;
using baraka.promo.Pages.Promos;
using baraka.promo.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baraka.promo.Core
{
    public class GetPromoByName
    {
        public class Command : IRequest<ApiBaseResultModel<PromoApiResultModel>>
        {
            public Command(OrderPromoModel model)
            {
                Model = model;
            }
            public OrderPromoModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<PromoApiResultModel>>
        {
            readonly ApplicationDbContext _db;
            readonly DeliveryDbContext _ddb;
            readonly ILogger<GetPromoByName> _logger;
            readonly CheckSegment _segment;
            public Handler(ApplicationDbContext db, DeliveryDbContext ddb, ILogger<GetPromoByName> logger, CheckSegment segment)
            {
                _db = db;
                _ddb = ddb;
                _logger = logger;
                _segment = segment;
            }

            public async Task<ApiBaseResultModel<PromoApiResultModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                var model = request.Model;

                if (model != null)
                {
                    try
                    {
                        var promo = _db.Promos.Where(a => (!a.EndTime.HasValue || a.EndTime > DateTime.Now) && !a.IsUnique).FirstOrDefault(a => a.Name == model.PromoName);
                        PromoChildValue unique_promo = null;

                        if (promo == null)
                        {
                            unique_promo = _db.PromoChildValues.FirstOrDefault(x => x.Name == model.PromoName && !x.TimeOfUse.HasValue);

                            if (unique_promo != null)
                            {
                                promo = _db.Promos.FirstOrDefault(x => x.Id == unique_promo.PromoId && (!x.EndTime.HasValue || x.EndTime > DateTime.Now));
                            }
                        }

                        if (promo != null && promo.IsDeleted == false && promo.IsActive == true)
                        {
                            var regions = _db.PromoRegions.Where(a => a.PromoId == promo.Id).ToList();
                            var restaurants = _db.PromoRestaurants.Where(a => a.PromoId == promo.Id).ToList();
                            var promo_arbitrations = _db.PromoArbitrations.Where(x => x.PromoId == promo.Id).ToList();

                            bool isRegion = regions != null && regions.Count > 0 ? regions.Any(a => a.RegionId == model.RegionId) : true;
                            bool isRestaurant = restaurants != null && restaurants.Count > 0 ? restaurants.Any(a => a.RestaurantId == model.RestaurantId) : true;

                            bool isEnoughAmount = (promo.MinOrderAmount != null ? model.OrderAmount >= promo.MinOrderAmount : true) == (promo.MaxOrderAmount != null ? model.OrderAmount <= promo.MaxOrderAmount : true);
                            bool isStarted = promo.StartTime < DateTime.Now;
                            bool isEnded = promo.EndTime == null || promo.EndTime > DateTime.Now ? true : false;
                            var isSegment = promo.Type == PromoType.Segment ? _segment.IsSegmentCompatible(model.ClientPhone, (int)promo.SegmentId) : (true, null);

                            bool usedCount = UsedPromoCount(model.ClientPhone, promo.Id, promo.MaxCount, promo.Type);

                            bool has_promo_arbitrations = IsHasProductArbitrations(model.Products, promo_arbitrations);

                            if (isRegion && isRestaurant && usedCount && isEnoughAmount && isStarted && isEnded && isSegment.Item1 && !has_promo_arbitrations)
                            {
                                var freeProducts = new List<FreeProductModel>();
                                if (promo.View == PromoView.FreeProduct)
                                {
                                    freeProducts = _db.PromoProducts.Where(a => a.PromoId == promo.Id && !a.Discount.HasValue)
                                                      .Select(s => new FreeProductModel
                                                      {
                                                          ProductId = s.ProductId,
                                                          Count = s.Count,
                                                      }).ToList();
                                }

                                var discountProducts = new List<DiscountProductModel>();
                                if (promo.View == PromoView.ProductDiscount)
                                {
                                    discountProducts = _db.PromoProducts.Where(a => a.PromoId == promo.Id && a.Discount.HasValue)
                                                                .Select(s => new DiscountProductModel
                                                                {
                                                                    ProductId = s.ProductId,
                                                                    Discount = s.Discount.Value,
                                                                    Count = s.Count,
                                                                }).ToList();
                                }

                                var promotionalProducts = new PromotionalProductModel();
                                if (promo.View == PromoView.PromotionalProduct)
                                {
                                    var promoProducts = _db.PromoProducts.Where(a => a.PromoId == promo.Id).ToList();
                                    var productFeatures = _db.PromoProductFeatures.Where(a => a.PromoId == promo.Id).ToList();

                                    promotionalProducts = new PromotionalProductModel()
                                    {
                                        ProductIds = promoProducts.Select(a => new FreeProductModel()
                                        {
                                            ProductId = a.ProductId,
                                            Count = a.Count
                                        }).ToList(),

                                        FreeProducts = productFeatures.Select(a => new FreeProductModel()
                                        {
                                            ProductId = a.ProductId,
                                            Count = a.Count
                                        }).ToList()
                                    };
                                }

                                var result = new PromoApiResultModel()
                                {
                                    Id = promo.Id,
                                    Name = promo.Name,
                                    OrderDiscount = promo.OrderDiscount,
                                    MaxCount = promo.MaxCount,
                                    MinOrderAmount = promo.MinOrderAmount,
                                    MaxOrderAmount = promo.MaxOrderAmount,
                                    PromoType = promo.Type,
                                    PromoView = promo.View,
                                    FreeProduct = freeProducts,
                                    ProductDiscount = discountProducts,
                                    PromotionalProduct = promotionalProducts
                                };

                                if (unique_promo != null) result.Id = unique_promo.Id;

                                return new ApiBaseResultModel<PromoApiResultModel>(result);
                            }
                            else
                            {
                                var descriptions = new StringBuilder();

                                if (!isRegion)
                                    descriptions.AppendLine("Акция недоступна в этом регионе!");
                                if (!isRestaurant)
                                    descriptions.AppendLine("Акция недоступна в этом ресторане!");
                                if (!isEnoughAmount)
                                    descriptions.AppendLine("Сумма заказа не соответствует критериям промокода!");
                                if (!isStarted)
                                    descriptions.AppendLine("Дата начало промокода не настал!");
                                if (!isEnded)
                                    descriptions.AppendLine("Дата использование промокода истекла!");
                                if (!isSegment.Item1)
                                    descriptions.AppendLine(isSegment.Item2);
                                if (!usedCount)
                                    descriptions.AppendLine("Эта акция уже использована!");
                                if (has_promo_arbitrations)
                                    descriptions.AppendLine("Заказ имеет другие акции!");

                                return new ApiBaseResultModel<PromoApiResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_REQUIREMENTS, descriptions.ToString(), null));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "GetPromoByName");
                        return new ApiBaseResultModel<PromoApiResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL, ex.Message));
                    }
                }

                return new ApiBaseResultModel<PromoApiResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_NOT_FOUND));
            }

            private bool UsedPromoCount(string phone, long promoId, int? maxCount, PromoType type)
            {
                var user = _db.PromoClients.Where(a => a.Phone == phone && a.PromoId == promoId).Select(s => new { s.TimeOfUse }).ToList();

                if (type.HasFlag(PromoType.Personal) && user.Count == 0)
                    return false;

                if (!maxCount.HasValue)
                    return true;

                if (user.Count == 0)
                {
                    if (type.HasFlag(PromoType.All))
                        return true;
                    else if (type.HasFlag(PromoType.Personal))
                        return false;
                }

                var count = user.Where(a => a.TimeOfUse.HasValue).Count();
                if (count >= maxCount)
                    return false;
                else
                    return true;
            }

            bool IsHasProductArbitrations(List<Guid> order_products, List<PromoArbitration> promo_arbitrations)
            {
                if (order_products == null || order_products.Count == 0) return false;

                if (promo_arbitrations == null || promo_arbitrations.Count == 0) return false;

                foreach (var item in order_products)
                {
                    if (promo_arbitrations.Any(x => x.ProductId == item)) return true;
                }

                return false;
            }
        }
    }
}
