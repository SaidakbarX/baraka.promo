using baraka.promo.Core.Cards;
using baraka.promo.Data.Loyalty;
using baraka.promo.Data;
using baraka.promo.Models.Cards;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using baraka.promo.Data.PromoEntities;
using baraka.promo.Models.PromoModels.NewPromoModels;

namespace baraka.promo.Core.PromoMethods
{
    public class AddUpdatePromoGroup
    {

        public class Command : IRequest<ApiBaseResultModel<PromoGroupResult>>
        {
            public Command(PromoGroupModel model)
            {
                Model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public PromoGroupModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<PromoGroupResult>>
        {
            readonly ILogger<AddCard> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<AddCard> logger, ApplicationDbContext db, ICurrentUser currentUser)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
            }

            public async Task<ApiBaseResultModel<PromoGroupResult>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null) return new ApiBaseResultModel<PromoGroupResult>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));
                    if (!_current_user.IsAdmin()) return new ApiBaseResultModel<PromoGroupResult>(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));

                    var model = request.Model;

                    var promoGroup = _db.PromoGroups.FirstOrDefault(x => x.Id == model.Id);
                    int promoGroupId = promoGroup?.Id ?? 0;
                    if (promoGroup == null)
                    {
                        int maxOrder = 0;
                        if (_db.PromoGroups.Count(c => !c.IsDeleted) > 0)
                            maxOrder = _db.PromoGroups.Where(w => !w.IsDeleted).Max(x => x.Order);
                        maxOrder++;
                        promoGroup = new(maxOrder, model.Name, user, model.Description);
                        await _db.PromoGroups.AddAsync(promoGroup, cancellationToken);
                    }
                    else
                        promoGroup.Update(model.Name, model.Description, user);
                    await _db.SaveChangesAsync(cancellationToken);
                    promoGroupId = promoGroup.Id;

                    return new ApiBaseResultModel<PromoGroupResult>(new PromoGroupResult { Id = promoGroupId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<PromoGroupResult>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
