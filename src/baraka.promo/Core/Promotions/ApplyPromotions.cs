using baraka.promo.Models;
using MediatR;
using baraka.promo.Models.OrderApiModel;
using baraka.promo.Data;
using baraka.promo.Services;
using baraka.promo.Utils;
using Microsoft.Extensions.Caching.Memory;
using baraka.promo.Delivery;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace baraka.promo.Core.Promotions
{
    public class ApplyPromotions
    {
        public class Command : IRequest<ApiBaseResultModel<PreorderResultModel>>
        {
            public Command(PreorderModel model, int integrationId)
            {
                Model = model ?? throw new ArgumentNullException(nameof(model));
                IntegrationId = integrationId;
            }

            public PreorderModel Model { get; set; }
            public int IntegrationId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<PreorderResultModel>>
        {
            readonly ILogger<ApplyPromotions> _logger;
            readonly ApplicationDbContext _db;
            readonly DeliveryDbContext _delivery_db;
            readonly IMemoryCache _memory_cache;
            readonly IMediator _mediator;

            public Handler(ILogger<ApplyPromotions> logger, ApplicationDbContext db, 
                IMemoryCache memoryCache, DeliveryDbContext deliveryDbContext, IMediator mediator)
            {
                _logger = logger;
                _db = db;
                _memory_cache = memoryCache;
                _delivery_db = deliveryDbContext;
                _mediator = mediator;
            }

            public async Task<ApiBaseResultModel<PreorderResultModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    //var user = _current_user.GetCurrentUserName();
                    //if (user == null) return new ApiBaseResultModel<PreorderResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));

                    var model = request.Model;

                    model.order.totalDiscount = null;
                    model.order.deliveryDiscount = null;

                    foreach (var item in model.order.lines)
                    {
                        item.discountedPriceOfLine = 0;
                    }

                    int integration_id = request.IntegrationId;

                    PreorderResultModel result = new()
                    {
                        Customer = new CustomerModel { MobilePhone = model.customer.MobilePhone },
                        Order = model.order,
                    };

                    List<OrderItemModel> promotions_items = new List<OrderItemModel>();

                    string client_phone = Regex.Replace(model.customer.MobilePhone ?? "", @"\D", "");

                    string cache_key = TextConstants.PromotionsCacheKey;
                    if (!_memory_cache.TryGetValue(cache_key, out List<Promo> promotions))
                    {
                        promotions = _db.Promos.Where(x => x.IsPromotion && x.IsActive && !x.IsDeleted && x.StartTime < DateTime.Now &&
                        (!x.EndTime.HasValue || x.EndTime > DateTime.Now)).ToList();
                        _memory_cache.Set(cache_key, promotions, DateTimeOffset.Now.AddMinutes(1));
                    }

                    foreach (var promotion in promotions)
                    {
                        string cache_key_regions = TextConstants.PromotionsRegionsCacheKey + promotion.Id;
                        if (!_memory_cache.TryGetValue(cache_key_regions, out List<PromoRegion> promo_regions))
                        {
                            promo_regions = _db.PromoRegions.Where(a => a.PromoId == promotion.Id).ToList();
                            _memory_cache.Set(cache_key_regions, promo_regions, DateTimeOffset.Now.AddMinutes(1));
                        }
                        string cache_key_restaurants = TextConstants.PromotionsRegionsCacheKey + promotion.Id;
                        if (!_memory_cache.TryGetValue(cache_key_restaurants, out List<PromoRestaurant> promo_restaurants))
                        {
                            promo_restaurants = _db.PromoRestaurants.Where(a => a.PromoId == promotion.Id).ToList();
                            _memory_cache.Set(cache_key_restaurants, promo_restaurants, DateTimeOffset.Now.AddMinutes(1));
                        }
                        string cache_key_arbitration = TextConstants.PromotionsRegionsCacheKey + promotion.Id;
                        if (!_memory_cache.TryGetValue(cache_key_arbitration, out List<PromoArbitration> promo_arbitrations))
                        {
                            promo_arbitrations = _db.PromoArbitrations.Where(x => x.PromoId == promotion.Id).ToList();
                            _memory_cache.Set(cache_key_arbitration, promo_arbitrations, DateTimeOffset.Now.AddMinutes(1));
                        }
                        string cache_key_integrations = TextConstants.PromotionsIntegrationsCacheKey + promotion.Id;
                        if (!_memory_cache.TryGetValue(cache_key_integrations, out List<PromoIntegration> promo_integrations))
                        {
                            promo_integrations = _db.PromoIntegrations.Where(a => a.PromoId == promotion.Id).ToList();
                            _memory_cache.Set(cache_key_integrations, promo_integrations, DateTimeOffset.Now.AddMinutes(1));
                        }

                        if (promo_regions.Count > 0 && !promo_regions.Any(x => x.RegionId == model.order.customFields.RegionId)) continue;
                        if (promo_restaurants.Count > 0 && !promo_restaurants.Any(x => x.RestaurantId.ToString() == model.order.customFields.FilialDostavki)) continue;
                        if (promo_integrations.Count > 0 && !promo_integrations.Any(x => x.IntegrationId == integration_id)) continue;

                        if (promotion.MinOrderAmount.HasValue && promotion.MinOrderAmount.Value > model.order.totalCost)
                        {
                            if (promotion.View == PromoView.OrderMinSumPromotion)
                            {
                                var promo_product = _db.PromoProducts.Where(x => x.PromoId == promotion.Id).FirstOrDefault();
                                if (model.order.lines.Any(x => x.product.Ids.ProductExternalId == promo_product.ProductId))
                                {
                                    var promotion_lines = result.Order.lines.Where(x => x.product.Ids.ProductExternalId == promo_product.ProductId).ToList();
                                    foreach (var item in promotion_lines)
                                    {
                                        item.status = new OrderStatusModel { Ids = new StatusIdsModel { ExternalId = "Deleted" }, Reason = "" };
                                    }
                                }
                            }
                            continue;
                        }

                        if (promotion.MaxOrderAmount.HasValue && promotion.MaxOrderAmount.Value != 0 && promotion.MaxOrderAmount.Value < model.order.totalCost) continue;

                        if (promotion.Type == PromoType.Personal && !_db.PromoClients.Any(x => x.PromoId == promotion.Id && x.Phone == client_phone)) continue;

                        if (promotion.Type == PromoType.Segment)
                        {
                            string cache_key_segment = TextConstants.PromotionsSegmentInfoCacheKey + client_phone + promotion.SegmentId.Value;
                            if (!_memory_cache.TryGetValue(cache_key_segment, out bool is_segment))
                            {
                                is_segment = new CheckSegment(_delivery_db, _db).IsSegmentCompatible(client_phone, promotion.SegmentId.Value).isSuccess;
                                _memory_cache.Set(cache_key_segment, is_segment, DateTimeOffset.Now.AddMinutes(2));
                            }

                            if (!is_segment) continue;
                        }

                        if (promo_arbitrations.Count > 0)
                        {
                            bool is_continue = false;

                            foreach (var item in model.order.lines)
                            {
                                if (promo_arbitrations.Any(x => x.ProductId.ToString() == item.product.Ids.ProductExternalId))
                                {
                                    is_continue = true;
                                    break;
                                }
                            }

                            if (is_continue) continue;
                        }

                        if (promotion.View == PromoView.PromotionalProduct)
                        {
                            var promo_products = _db.PromoProducts.Where(x => x.PromoId == promotion.Id).ToList();
                            foreach (var item in model.order.lines)
                            {
                                if (promo_products.Any(x => x.ProductId == item.product.Ids.ProductExternalId && x.Count <= item.quantity))
                                {
                                    var promo_gift_products = _db.PromoProductFeatures.Where(x => x.PromoId == promotion.Id).ToList();
                                    foreach (var gift_product in promo_gift_products)
                                    {
                                        //var line = model.order.lines.FirstOrDefault(f => f.product.Ids.ProductExternalId == gift_product.ProductId);
                                        OrderItemModel new_item = new()
                                        {
                                            product = new OrderProductInfoModel { Ids = new ProductIdsModel { ProductExternalId = gift_product.ProductId } },
                                            quantity = gift_product.Count,
                                            status = new OrderStatusModel { Ids = new StatusIdsModel { ExternalId = "Create" }, Reason = "GIFT" },
                                            lineId = gift_product.ProductId,
                                            //basePricePerItem = line?.basePricePerItem ?? 0,
                                            //discountedPriceOfLine = line?.basePricePerItem ?? 0,
                                        };

                                        promotions_items.Add(new_item);
                                    }
                                }
                            }
                        }

                        else if (promotion.View == PromoView.FreeDelivery)
                        {
                            result.Order.deliveryDiscount = new() { DiscountId = promotion.Id, DiscountCost = model.order.deliveryCost };
                        }
                        else if (promotion.View == PromoView.OrderDiscount && promotion.OrderDiscount.HasValue)
                        {
                            if (result.Order.totalDiscount == null) result.Order.totalDiscount = new() { DiscountId = promotion.Id, DiscountCost = model.order.totalCost * promotion.OrderDiscount.Value / 100 };
                            else result.Order.totalDiscount.DiscountCost += model.order.totalCost * promotion.OrderDiscount.Value / 100;
                        }
                        else if (promotion.View == PromoView.OrderDiscountAmount && promotion.OrderDiscount.HasValue)
                        {
                            if (result.Order.totalDiscount == null) result.Order.totalDiscount = new() { DiscountId = promotion.Id, DiscountCost = promotion.OrderDiscount.Value * 100 };
                            else result.Order.totalDiscount.DiscountCost += promotion.OrderDiscount.Value * 100;
                        }
                        else if (promotion.View == PromoView.ProductDiscount)
                        {
                            var promo_products = _db.PromoProducts.Where(x => x.PromoId == promotion.Id).ToList();

                            foreach (var product_discount in promo_products)
                            {
                                if (model.order.lines.Any(x => x.product.Ids.ProductExternalId == product_discount.ProductId && x.quantity >= product_discount.Count) && product_discount.Discount.HasValue)
                                {
                                    var discount_line = model.order.lines.FirstOrDefault(x => x.product.Ids.ProductExternalId == product_discount.ProductId && x.quantity >= product_discount.Count);
                                    discount_line.discountedPriceOfLine += discount_line.basePricePerItem * product_discount.Count * product_discount.Discount.Value / 100;
                                }
                            }
                        }
                        else if (promotion.View == PromoView.FreeProduct)
                        {
                            var promo_products = _db.PromoProducts.Where(x => x.PromoId == promotion.Id).ToList();
                            foreach (var gift_product in promo_products)
                            {
                                //var line = model.order.lines.FirstOrDefault(f => f.product.Ids.ProductExternalId == gift_product.ProductId);

                                OrderItemModel new_item = new()
                                {
                                    product = new OrderProductInfoModel { Ids = new ProductIdsModel { ProductExternalId = gift_product.ProductId } },
                                    quantity = gift_product.Count,
                                    status = new OrderStatusModel { Ids = new StatusIdsModel { ExternalId = "Create" }, Reason = "GIFT" },
                                    lineId = gift_product.ProductId,
                                    //basePricePerItem = line?.basePricePerItem ?? 0,
                                    //discountedPriceOfLine = line?.basePricePerItem ?? 0,

                                };

                                promotions_items.Add(new_item);
                            }
                        }
                    }                   

                    if(model.order.coupons?.Count > 0)
                    {
                        var coupons = new List<CouponModel>();

                        foreach (var promo in model.order.coupons)
                        {
                            try
                            {
                                if (coupons.Any(x => x.Code == promo.Code)) continue;

                                CouponModel new_coupon = new();

                                OrderPromoModel promo_model = new OrderPromoModel
                                {
                                    RegionId = model.order.customFields.RegionId,
                                    RestaurantId = !string.IsNullOrEmpty(model.order.customFields.FilialDostavki) ? Guid.Parse(model.order.customFields.FilialDostavki) : Guid.Empty,
                                    ClientPhone = client_phone,
                                    PromoName = promo.Code,
                                    OrderAmount = (long)model.order.totalCost,
                                    Products = new List<Guid>(),
                                };

                                foreach (var item in model.order.lines)
                                {
                                    promo_model.Products.Add(Guid.Parse(item.product.Ids.ProductExternalId));
                                }

                                var command = new GetPromoByName.Command(promo_model);
                                var promo_result = await _mediator.Send(command);

                                if(promo_result != null && promo_result.Data != null)
                                {
                                    var promo_info = promo_result.Data;

                                    new_coupon.Data = promo_info;
                                    new_coupon.IsUsed = true;
                                    new_coupon.Code = promo.Code;  

                                    if (promo_info.PromoView == PromoView.OrderMinSumPromotion)
                                    {
                                        var promo_product = _db.PromoProducts.Where(x => x.PromoId == promo_info.Id).FirstOrDefault();
                                        if (model.order.lines.Any(x => x.product.Ids.ProductExternalId == promo_product.ProductId))
                                        {
                                            var promotion_lines = result.Order.lines.Where(x => x.product.Ids.ProductExternalId == promo_product.ProductId).ToList();
                                            foreach (var item in promotion_lines)
                                            {
                                                item.status = new OrderStatusModel { Ids = new StatusIdsModel { ExternalId = "Deleted" }, Reason = "" };
                                            }
                                        }
                                    }
                                    else if (promo_info.PromoView == PromoView.PromotionalProduct)
                                    {
                                        var promo_products = _db.PromoProducts.Where(x => x.PromoId == promo_info.Id).ToList();
                                        foreach (var item in model.order.lines)
                                        {
                                            if (promo_products.Any(x => x.ProductId == item.product.Ids.ProductExternalId && x.Count <= item.quantity))
                                            {
                                                var promo_gift_products = _db.PromoProductFeatures.Where(x => x.PromoId == promo_info.Id).ToList();
                                                foreach (var gift_product in promo_gift_products)
                                                {
                                                    //var line = model.order.lines.FirstOrDefault(f => f.product.Ids.ProductExternalId == gift_product.ProductId);
                                                    OrderItemModel new_item = new()
                                                    {
                                                        product = new OrderProductInfoModel { Ids = new ProductIdsModel { ProductExternalId = gift_product.ProductId } },
                                                        quantity = gift_product.Count,
                                                        status = new OrderStatusModel { Ids = new StatusIdsModel { ExternalId = "Create" }, Reason = "GIFT" },
                                                        lineId = gift_product.ProductId,
                                                        //basePricePerItem = line?.basePricePerItem ?? 0,
                                                        //discountedPriceOfLine = line?.basePricePerItem ?? 0,
                                                    };

                                                    promotions_items.Add(new_item);
                                                }
                                            }
                                        }
                                    }

                                    else if (promo_info.PromoView == PromoView.FreeDelivery)
                                    {
                                        result.Order.deliveryDiscount = new() { DiscountId = promo_info.Id, DiscountCost = model.order.deliveryCost };
                                    }
                                    else if (promo_info.PromoView == PromoView.OrderDiscount && promo_info.OrderDiscount.HasValue)
                                    {
                                        if (result.Order.totalDiscount == null) result.Order.totalDiscount = new() { DiscountId = promo_info.Id, DiscountCost = model.order.totalCost * promo_info.OrderDiscount.Value / 100 };
                                        else result.Order.totalDiscount.DiscountCost += model.order.totalCost * promo_info.OrderDiscount.Value / 100;
                                    }
                                    else if (promo_info.PromoView == PromoView.OrderDiscountAmount && promo_info.OrderDiscount.HasValue)
                                    {
                                        if (result.Order.totalDiscount == null) result.Order.totalDiscount = new() { DiscountId = promo_info.Id, DiscountCost = promo_info.OrderDiscount.Value * 100 };
                                        else result.Order.totalDiscount.DiscountCost += promo_info.OrderDiscount.Value * 100;
                                    }
                                    else if (promo_info.PromoView == PromoView.ProductDiscount)
                                    {
                                        var promo_products = _db.PromoProducts.Where(x => x.PromoId == promo_info.Id).ToList();

                                        foreach (var product_discount in promo_products)
                                        {
                                            if (model.order.lines.Any(x => x.product.Ids.ProductExternalId == product_discount.ProductId && x.quantity >= product_discount.Count) && product_discount.Discount.HasValue)
                                            {
                                                var discount_line = model.order.lines.FirstOrDefault(x => x.product.Ids.ProductExternalId == product_discount.ProductId && x.quantity >= product_discount.Count);
                                                discount_line.discountedPriceOfLine += discount_line.basePricePerItem * product_discount.Count * product_discount.Discount.Value / 100;
                                            }
                                        }
                                    }
                                    else if (promo_info.PromoView == PromoView.FreeProduct)
                                    {
                                        var promo_products = _db.PromoProducts.Where(x => x.PromoId == promo_info.Id).ToList();
                                        foreach (var gift_product in promo_products)
                                        {
                                            //var line = model.order.lines.FirstOrDefault(f => f.product.Ids.ProductExternalId == gift_product.ProductId);

                                            OrderItemModel new_item = new()
                                            {
                                                product = new OrderProductInfoModel { Ids = new ProductIdsModel { ProductExternalId = gift_product.ProductId } },
                                                quantity = gift_product.Count,
                                                status = new OrderStatusModel { Ids = new StatusIdsModel { ExternalId = "Create" }, Reason = "GIFT" },
                                                lineId = gift_product.ProductId,
                                                //basePricePerItem = line?.basePricePerItem ?? 0,
                                                //discountedPriceOfLine = line?.basePricePerItem ?? 0,

                                            };

                                            promotions_items.Add(new_item);
                                        }
                                    }
                                }
                                else
                                {
                                    new_coupon.IsUsed = false;
                                    new_coupon.Code = promo.Code;
                                    new_coupon.Reason = promo_result.Error?.Message;
                                }

                                coupons.Add(new_coupon);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, ex.Message);
                            }
                        }

                        result.Order.coupons = coupons;
                    }

                    if (promotions_items.Count > 0) result.Order.lines.AddRange(promotions_items);

                    return new ApiBaseResultModel<PreorderResultModel>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"ApplyPromotions error -> {ex.Message}");
                    return new ApiBaseResultModel<PreorderResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
