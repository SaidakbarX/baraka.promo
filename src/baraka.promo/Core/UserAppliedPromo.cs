using baraka.promo.Data;
using baraka.promo.Delivery;
using baraka.promo.Models;
using baraka.promo.Models.OrderApiModel;
using baraka.promo.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace baraka.promo.Core
{
    public class UserAppliedPromo
    {
        public class Command : IRequest<ApiBaseResultModel>
        {
            public Command(AppliedPromoModel model)
            {
                Model = model;
            }
            public AppliedPromoModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel>
        {
            readonly ApplicationDbContext _db;
            readonly DeliveryDbContext _ddb;
            readonly ILogger<UserAppliedPromo> _logger;
            readonly CheckSegment _segment;
            public Handler(ApplicationDbContext db, ILogger<UserAppliedPromo> logger, DeliveryDbContext ddb, CheckSegment isSegment)
            {
                _db = db;
                _logger = logger;
                _ddb = ddb;
                _segment = isSegment;
            }

            public async Task<ApiBaseResultModel> Handle(Command request, CancellationToken cancellationToken)
            {
                var model = request.Model;

                if (model != null)
                {
                    try
                    {
                        var promo = _db.Promos.FirstOrDefault(a => a.Id == model.PromoId && a.IsDeleted == false && a.IsActive == true);

                        PromoChildValue unique_promo = null;

                        if (promo == null)
                        {
                            unique_promo = _db.PromoChildValues.FirstOrDefault(x => x.Id == model.PromoId && !x.TimeOfUse.HasValue);

                            if (unique_promo != null)
                            {
                                promo = _db.Promos.FirstOrDefault(x => x.Id == unique_promo.PromoId && x.IsDeleted == false && x.IsActive == true);
                            }
                        }

                        if (promo != null)
                        {
                            var regions = _db.PromoRegions.Where(a => a.PromoId == promo.Id).Select(s => s.RegionId).ToList();
                            var restaurants = _db.PromoRestaurants.Where(a => a.PromoId == promo.Id).ToList();
                            var promoTotalUsedCount = _db.PromoClients.Where(a => a.PromoId == promo.Id && a.TimeOfUse.HasValue).Count();
                            var promo_arbitrations = _db.PromoArbitrations.Where(x=>x.PromoId == promo.Id).ToList();

                            bool isRegion = regions.Count <= 0 || regions.Contains(model.RegionId);
                            bool isRestaurant = restaurants.Count <= 0 || restaurants.Any(a => a.RestaurantId == model.RestaurantId);
                            bool isEnoughAmount = (promo.MinOrderAmount != null ? model.OrderAmount >= promo.MinOrderAmount : true) == (promo.MaxOrderAmount != null ? model.OrderAmount <= promo.MaxOrderAmount : true);
                            bool isStarted = promo.StartTime < DateTime.Now;
                            bool isEnded = promo.EndTime == null || promo.EndTime > DateTime.Now ? true : false;
                            bool isMaxUsed = !promo.TotalCount.HasValue || promo.TotalCount > promoTotalUsedCount;
                            var isSegment = promo.Type == PromoType.Segment ? _segment.IsSegmentCompatible(model.ClientPhone, (int)promo.SegmentId) : (true, null);

                            bool usedCount = UsedPromoCount(model.ClientPhone, promo.Id, promo.MaxCount, promo.Type);

                            bool has_promo_arbitrations = IsHasProductArbitrations(model.Products, promo_arbitrations);

                            if ((promo.Type == PromoType.All || promo.Type == PromoType.Segment) && isRegion && isRestaurant && usedCount && isEnoughAmount && isStarted && isEnded && isMaxUsed && !has_promo_arbitrations)
                            {
                                if(promo.Type == PromoType.Segment && !isSegment.Item1)
                                    return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_REQUIREMENTS, isSegment.Item2));

                                await _db.PromoClients.AddAsync(new PromoClient()
                                {
                                    OrderId = model.OrderId,
                                    Phone = model.ClientPhone,
                                    PromoId = model.PromoId,
                                    TimeOfUse = DateTime.Now
                                });
                                await _db.SaveChangesAsync();

                                if (unique_promo != null)
                                {
                                    unique_promo.ClientPhone = model.ClientPhone;
                                    unique_promo.TimeOfUse = DateTime.Now;

                                    await _db.SaveChangesAsync();
                                }

                                promoTotalUsedCount++;

                                if (promo.TotalCount.HasValue && promo.TotalCount == promoTotalUsedCount)
                                {
                                    promo.IsActive = false;
                                    await _db.SaveChangesAsync();
                                }
                                return new ApiBaseResultModel();
                            }
                            else if (promo.Type == PromoType.Personal && isRegion && isRestaurant && usedCount && isEnoughAmount && isStarted && isEnded && isMaxUsed && !has_promo_arbitrations)
                            {
                                var user = await _db.PromoClients.FirstOrDefaultAsync(a => a.PromoId == promo.Id && a.Phone == model.ClientPhone && a.TimeOfUse == null);
                                if (user != null)
                                {
                                    user.OrderId = model.OrderId;
                                    user.TimeOfUse = DateTime.Now;
                                    await _db.SaveChangesAsync();
                                }
                                else
                                {
                                    await _db.PromoClients.AddAsync(new PromoClient()
                                    {
                                        OrderId = model.OrderId,
                                        Phone = model.ClientPhone,
                                        PromoId = model.PromoId,
                                        TimeOfUse = DateTime.Now,
                                    });
                                    await _db.SaveChangesAsync();
                                }

                                if (unique_promo != null)
                                {
                                    unique_promo.ClientPhone = model.ClientPhone;
                                    unique_promo.TimeOfUse = DateTime.Now;

                                    await _db.SaveChangesAsync();
                                }

                                promoTotalUsedCount++;

                                if (promo.TotalCount.HasValue && promo.TotalCount == promoTotalUsedCount)
                                {
                                    promo.IsActive = false;
                                    await _db.SaveChangesAsync();
                                }
                                return new ApiBaseResultModel();
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
                                if(has_promo_arbitrations) 
                                    descriptions.AppendLine("Заказ имеет другие акции!");

                                if (isMaxUsed == false)
                                {
                                    promo.IsActive = false;
                                    await _db.SaveChangesAsync();
                                    descriptions.AppendLine("Эта акция уже использована!");
                                }

                                return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_REQUIREMENTS, descriptions.ToString(), null));
                            }
                        }
                        else
                        {
                            return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_NOT_FOUND));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "UserAppliedPromo");
                        return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL, ex.Message));
                    }
                }
                else
                    return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_NOT_FOUND));
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
                if(order_products == null || order_products.Count == 0) return false;

                if(promo_arbitrations == null || promo_arbitrations.Count == 0) return false;

                foreach (var item in order_products)
                {
                    if(promo_arbitrations.Any(x=>x.ProductId == item)) return true;
                }

                return false;
            }
        }
    }
}
