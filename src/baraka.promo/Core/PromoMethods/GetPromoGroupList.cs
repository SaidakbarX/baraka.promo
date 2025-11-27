using baraka.promo.Core.Cards;
using baraka.promo.Data;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Models.PromoModels.NewPromoModels;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baraka.promo.Core.PromoMethods
{
    public class GetPromoGroupList
    {
        public class Command : IRequest<ApiBaseResultModel<ListBaseModel<PromoGroupModel>>>
        {
            public Command(PromoFilter model)
            {
                Model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public PromoFilter Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<ListBaseModel<PromoGroupModel>>>
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

            public async Task<ApiBaseResultModel<ListBaseModel<PromoGroupModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                ListBaseModel<PromoGroupModel> result = new();
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null) return new ApiBaseResultModel<ListBaseModel<PromoGroupModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));
                    if (!_current_user.IsAdmin()) return new ApiBaseResultModel<ListBaseModel<PromoGroupModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));

                    var Model = request.Model;
                    var query = _db.PromoGroups.Where(w => !w.IsDeleted);

                    if (!string.IsNullOrEmpty(Model.SearchText))
                        query = query.Where(w => w.Name.Contains(Model.SearchText));


                    query = query.OrderByDescending(o => o.ModifiedTime);

                    int total = query.Count();



                    result.List = query.Select(s => new PromoGroupModel()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        Order = s.Order,
                        ModifiedBy = s.ModifiedBy,

                    }).Skip(Model.Skip).Take(Model.Take).ToList() ?? new();



                    if (Model.Skip == 0)
                    {
                        result.List.Add(new PromoGroupModel()
                        {
                            Id = 0,
                            Name = "Не сгруппированные промо",
                            Description = "Промоакции без группы",
                            Order = 0,
                            MemberCount = 0,
                            ModifiedBy = "System",
                        });
                    }


                    result.List = result.List.ToList();

                    result.Total = total;

                    return new ApiBaseResultModel<ListBaseModel<PromoGroupModel>>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<ListBaseModel<PromoGroupModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
