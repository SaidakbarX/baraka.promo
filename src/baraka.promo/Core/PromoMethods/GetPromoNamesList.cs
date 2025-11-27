using baraka.promo.Core.Cards;
using baraka.promo.Data;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Models.PromoModels.NewPromoModels;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;

namespace baraka.promo.Core.PromoMethods
{
    public class GetPromoNamesList
    {
        public class Command : IRequest<ApiBaseResultModel<ListBaseModel<PromoNameModel>>>
        {
            public Command(FilterModel model)
            {
                Model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public FilterModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<ListBaseModel<PromoNameModel>>>
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

            public async Task<ApiBaseResultModel<ListBaseModel<PromoNameModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                ListBaseModel<PromoNameModel> result = new();
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null) return new ApiBaseResultModel<ListBaseModel<PromoNameModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));
                    if (!_current_user.IsAdmin()) return new ApiBaseResultModel<ListBaseModel<PromoNameModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));

                    var model = request.Model;
                    var query = _db.Promos.Where(w => !w.IsDeleted && (w.EndTime == null || w.EndTime > DateTime.Now) && w.IsActive);

                    if (!string.IsNullOrEmpty(model.SearchText))
                        query = query.Where(w => w.Name.Contains(model.SearchText));

                    query = query.OrderByDescending(o => o.StartTime);

                    int total = query.Count();

                    result.List = query.Select(s => new PromoNameModel()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Type = s.Type,
                        View = s.View,
                    }).Skip(model.Skip).Take(model.Take).ToList() ?? new();

                    result.Total = total;

                    return new ApiBaseResultModel<ListBaseModel<PromoNameModel>>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<ListBaseModel<PromoNameModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
