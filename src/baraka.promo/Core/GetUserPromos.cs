using baraka.promo.Data;
using baraka.promo.Models;
using baraka.promo.Models.OrderApiModel;
using baraka.promo.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baraka.promo.Core
{
    public class GetUserPromos
    {
        public class Command : IRequest<ApiBaseResultModel<List<PromoApiResultModel>>>
        {
            public Command(string phone)
            {
                Phone = phone;
            }
            public string Phone { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<List<PromoApiResultModel>>>
        {
            readonly ApplicationDbContext _db;
            public Handler(ApplicationDbContext db)
            {
                _db = db;
            }
            public async Task<ApiBaseResultModel<List<PromoApiResultModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var phone = request.Phone;

                    var result = new List<PromoApiResultModel>();

                    var promoIds = _db.PromoClients.Where(w => !w.TimeOfUse.HasValue && w.Phone == phone)
                        .Select(s => s.PromoId).ToList();
                    if (promoIds.Count > 0)
                    {
                        var time = DateTime.Now;
                        var promos = await _db.Promos.Where(w => !w.IsDeleted && w.StartTime <= time
                        && (!w.EndTime.HasValue || w.EndTime >= time)
                        && promoIds.Contains(w.Id) && !w.IsUnique).ToListAsync();

                        foreach (var item in promos)
                        {
                            //bool usedCount = UsedPromoCount(phone, item.Id, item.MaxCount, item.Type);
                            //if (usedCount && item.Type == PromoType.All && item.IsDeleted == false)
                            //{
                            //    var freeProducts = await _db.PromoProducts.Where(a => a.PromoId == item.Id).ToListAsync();
                            //    var discountProducts = await _db.PromoProducts.Where(a => a.PromoId == item.Id).ToListAsync();

                            //    result.Add(new PromoApiResultModel()
                            //    {
                            //        Id = item.Id,
                            //        Name = item.Name,
                            //        MaxCount = item.MaxCount,
                            //        MinOrderAmount = item.MinOrderAmount,
                            //        MaxOrderAmount = item.MaxOrderAmount,
                            //        OrderDiscount = item.OrderDiscount,
                            //        PromoView = item.View,
                            //        PromoType = item.Type,
                            //        FreeProduct = freeProducts,
                            //        ProductDiscount = discountProducts
                            //    });
                            //}
                            //else if (usedCount && item.Type == PromoType.Personal && item.IsDeleted == false)
                            //{
                            var freeProducts = await _db.PromoProducts.Where(a => a.PromoId == item.Id && !a.Discount.HasValue)
                                .Select(s => new FreeProductModel
                                {
                                    ProductId = s.ProductId,
                                    Count = s.Count,
                                }).ToListAsync();
                            var discountProducts = await _db.PromoProducts.Where(a => a.PromoId == item.Id && a.Discount.HasValue)
                                .Select(s => new DiscountProductModel
                                {
                                    ProductId = s.ProductId,
                                    Discount = s.Discount.Value,
                                    Count = s.Count,
                                }).ToListAsync();

                            var arbitrations = await _db.PromoArbitrations.Where(a => a.PromoId == item.Id).Select(s => s.ProductId).ToListAsync();

                            result.Add(new PromoApiResultModel()
                            {
                                Id = item.Id,
                                Name = item.Name,
                                MaxCount = item.MaxCount,
                                MinOrderAmount = item.MinOrderAmount ,
                                MaxOrderAmount = item.MaxOrderAmount ,
                                OrderDiscount = item.OrderDiscount,
                                PromoView = item.View,
                                PromoType = item.Type,
                                FreeProduct = freeProducts,
                                ProductDiscount = discountProducts,
                                ArbitrationProducts = arbitrations,
                            });
                            //}
                        }
                    }

                    return new ApiBaseResultModel<List<PromoApiResultModel>>(result);
                }
                catch (Exception ex)
                {
                    return new ApiBaseResultModel<List<PromoApiResultModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL, ex.Message));
                }
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

        }


    }
}
