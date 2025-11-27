using baraka.promo.Data;
using baraka.promo.Delivery;
using baraka.promo.Models;
using baraka.promo.Models.OrderApiModel;
using baraka.promo.Utils;
using MediatR;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq.Dynamic;
using System.Text;

namespace baraka.promo.Core
{
    public class GetPromo
    {
        public class Command : IRequest<ApiBaseResultModel<OrderApiResultModel>>
        {
            public Command(OrderModel model)
            {
                Model = model;
            }
            public OrderModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<OrderApiResultModel>>
        {
            readonly ApplicationDbContext _db;
            readonly DeliveryDbContext _ddb;
            readonly ILogger<GetPromo> _logger;
            readonly CheckSegment _segment;
            public Handler(ApplicationDbContext db, DeliveryDbContext ddb, ILogger<GetPromo> logger, CheckSegment segment)
            {
                _db = db;
                _ddb = ddb;
                _logger = logger;
                _segment = segment;
            }
            public async Task<ApiBaseResultModel<OrderApiResultModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                var model = request.Model;

                if (model != null)
                {
                    try
                    {
                        var promos = (from p in _db.Promos
                                      where p.IsDeleted == false && p.IsActive == true && !p.IsUnique
                                      select new NewPromoModel()
                                      {
                                          Promo = new Promo()
                                          {
                                              Id = p.Id,
                                              Name = p.Name,
                                              MaxCount = p.MaxCount,
                                              StartTime = p.StartTime,
                                              EndTime = p.EndTime,
                                              MaxOrderAmount = p.MaxOrderAmount,
                                              MinOrderAmount = p.MinOrderAmount,
                                              OrderDiscount = p.OrderDiscount,
                                              IsDeleted = p.IsDeleted,
                                              Type = p.Type,
                                              View = p.View
                                          },
                                          PromoClients = _db.PromoClients.Where(a => a.PromoId == p.Id).ToList(),
                                          PromoProducts = _db.PromoProducts.Where(a => a.PromoId == p.Id).ToList(),
                                          PromoRestaurants = _db.PromoRestaurants.Where(a => a.PromoId == p.Id).ToList(),
                                          PromoRegions = _db.PromoRegions.Where(a => a.PromoId == p.Id).ToList(),
                                          PromoArbitrations = _db.PromoArbitrations.Where(a => a.PromoId == p.Id).ToList(),
                                      }).ToList();

                        var result = new OrderApiResultModel()
                        {
                            Promo = new List<PromoApiResultModel>()
                        };

                        foreach (var item in promos)
                        {
                            bool isRegion = item.PromoRegions != null && item.PromoRegions.Count > 0 ? item.PromoRegions.Any(a => a.RegionId == model.RegionId) : true;
                            bool isRestaurant = item.PromoRestaurants != null && item.PromoRestaurants.Count > 0 ? item.PromoRestaurants.Any(a => a.RestaurantId == model.RestaurantId) : true;
                            bool usedCount = UsedPromoCount(model.ClientPhone, item.Promo.Id, item.Promo.MaxCount, item.Promo.Type);
                            bool isEnoughAmount = (item.Promo.MinOrderAmount != null ? model.OrderAmount >= item.Promo.MinOrderAmount : true) == (item.Promo.MaxOrderAmount != null ? model.OrderAmount  <= item.Promo.MaxOrderAmount : true);
                            bool isStarted = item.Promo.StartTime < DateTime.Now;
                            bool isEnded = item.Promo.EndTime == null || item.Promo.EndTime > DateTime.Now ? true : false;
                            var isSegment = item.Promo.Type == PromoType.Segment ? _segment.IsSegmentCompatible(model.ClientPhone, (int)item.Promo.SegmentId) : (true, null);

                            bool has_promo_arbitrations = IsHasProductArbitrations(model.Products, item.PromoArbitrations);

                            if (isRegion && isRestaurant && usedCount && isEnoughAmount && isSegment.Item1 && !has_promo_arbitrations)
                            {
                                var freeProducts = item.PromoProducts?.Where(a => !a.Discount.HasValue)
                                    .Select(s => new FreeProductModel
                                    {
                                        ProductId = s.ProductId,
                                        Count = s.Count,
                                    }).ToList();

                                var discountProducts = item.PromoProducts?.Where(a => a.Discount.HasValue)
                                    .Select(s => new DiscountProductModel
                                    {
                                        ProductId = s.ProductId,
                                        Discount = s.Discount.Value,
                                        Count = s.Count,
                                    }).ToList();

                                result.Promo?.Add(new PromoApiResultModel()
                                {
                                    Id = item.Promo.Id,
                                    Name = item.Promo.Name,
                                    PromoType = item.Promo.Type,
                                    PromoView = item.Promo.View,
                                    OrderDiscount = item.Promo.OrderDiscount,
                                    FreeProduct = freeProducts,
                                    ProductDiscount = discountProducts,
                                    MaxCount = item.Promo.MaxCount,
                                    MaxOrderAmount = item.Promo.MaxOrderAmount,
                                    MinOrderAmount = item.Promo.MinOrderAmount
                                });
                            }

                        }
                        return new ApiBaseResultModel<OrderApiResultModel>(result);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "GetPromo");
                        return new ApiBaseResultModel<OrderApiResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL, ex.Message));
                    }
                }

                return new ApiBaseResultModel<OrderApiResultModel>();
            }

            private bool UsedPromoCount(string phone, long promoId, int? maxCount, PromoType type)
            {
                if (type.HasFlag(PromoType.Personal))
                {
                    var personal = _db.PromoClients.Where(a => a.Phone == phone).ToList();
                    if(personal.Count == 0 || personal == null)
                        return false;
                }

                if (!maxCount.HasValue)
                    return true;

                var user = _db.PromoClients.Where(a => a.Phone == phone && a.PromoId == promoId).ToList();
                if (user == null || user.Count == 0)
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

            bool IsHasProductArbitrations(List<OrderProductModel> order_products, List<PromoArbitration> promo_arbitrations)
            {
                if (order_products == null || order_products.Count == 0) return false;

                if (promo_arbitrations == null || promo_arbitrations.Count == 0) return false;

                foreach (var item in order_products)
                {
                    if (promo_arbitrations.Any(x => x.ProductId == item.ProductId)) return true;
                }

                return false;
            }
        }

        private class NewPromoModel
        {
            public Promo Promo { get; set; }
            public List<PromoClient>? PromoClients { get; set; }
            public List<PromoProduct>? PromoProducts { get; set; }
            public List<PromoRegion>? PromoRegions { get; set; }
            public List<PromoRestaurant>? PromoRestaurants { get; set; }
            public List<PromoArbitration>? PromoArbitrations { get; set; }
        }
    }
}
