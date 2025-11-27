using baraka.promo.Data;
using baraka.promo.Models;
using baraka.promo.Models.OrderApiModel;
using baraka.promo.Models.PromoApi;
using baraka.promo.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace baraka.promo.Core
{
    public class AddClientPromo
    {
        public class Command : IRequest<ApiBaseResultModel>
        {
            public Command(AddClientPromoModel model)
            {
                Model = model;
            }
            public AddClientPromoModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel>
        {
            readonly ApplicationDbContext _db;
            readonly ILogger<UserAppliedPromo> _logger;
            public Handler(ApplicationDbContext db, ILogger<UserAppliedPromo> logger)
            {
                _db = db;
                _logger = logger;
            }

            public async Task<ApiBaseResultModel> Handle(Command request, CancellationToken cancellationToken)
            {
                var model = request.Model;

                if (model != null)
                {
                    try
                    {
                        Promo? promo = null;
                        if (model.PromoId > 0)
                            promo = _db.Promos.FirstOrDefault(a => a.Id == model.PromoId && a.IsDeleted == false);
                        else if (!string.IsNullOrEmpty(model.PromoCode))
                            promo = _db.Promos.FirstOrDefault(a => a.Name == model.PromoCode && a.IsDeleted == false);

                        if (promo != null)
                        {
                            bool isEnded = promo.EndTime == null || promo.EndTime > DateTime.Now ? true : false;
                            if (promo.Type != PromoType.Personal)
                            {
                                return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_NOT_PERSONAL));
                            }
                            else if (!isEnded)
                            {
                                return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_ENDED));
                            }
                            else if (promo.MaxCount.HasValue && promo.MaxCount <= _db.PromoClients.Where(w => w.PromoId == promo.Id).Count())
                            {
                                return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_MAX_USED_OR_NOT_FOUND));
                            }

                            await _db.PromoClients.AddAsync(new PromoClient()
                            {
                                Phone = model.Phone,
                                PromoId = promo.Id,
                            });
                            await _db.SaveChangesAsync();
                            return new ApiBaseResultModel();
                        }
                        else
                        {
                            return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PROMO_NOT_FOUND));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL, ex.Message));
                    }
                }

                return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_CHANGES_NOT_SAVED));
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
