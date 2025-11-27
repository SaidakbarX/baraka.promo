using baraka.promo.Core.Cards;
using baraka.promo.Data;
using baraka.promo.Models.PromoModels.NewPromoModels;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;

namespace baraka.promo.Core.PromoMethods
{
    public class DeletePromoGroup
    {

        public class Command : IRequest<ApiBaseResultModel>
        {
            public Command(int id)
            {
                Id = id;
            }

            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel>
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

            public async Task<ApiBaseResultModel> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));
                    if (!_current_user.IsAdmin()) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));


                    int Id = request.Id;
                    var promoGroup = _db.PromoGroups.FirstOrDefault(f => f.Id == Id);

                    if (promoGroup == null) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_NOT_FOUND));

                    promoGroup.Delete(user);
                    
                    var promos = _db.Promos.Where(w => w.GroupId == Id).ToList();
                    if (promos?.Count > 0)
                        promos.ForEach(f => f.GroupId = null);

                    await _db.SaveChangesAsync(cancellationToken);

                    return new ApiBaseResultModel();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
