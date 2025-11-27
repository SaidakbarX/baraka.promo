using baraka.promo.Core.Cards;
using baraka.promo.Data.PromoEntities;
using baraka.promo.Data;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models.PromoModels.NewPromoModels;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;

namespace baraka.promo.Core.PromoMethods
{
    public class GetPromosByGroupName
    {
        public class Command : IRequest<ApiBaseResultModel<ListBaseModel<PromoModel>>>
        {
            public Command(PromoFilter model)
            {
                Model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public PromoFilter Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<ListBaseModel<PromoModel>>>
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

            public async Task<ApiBaseResultModel<ListBaseModel<PromoModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                ListBaseModel<PromoModel> result = new();
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null) return new ApiBaseResultModel<ListBaseModel<PromoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));
                    if (!_current_user.IsAdmin()) return new ApiBaseResultModel<ListBaseModel<PromoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));

                    var model = request.Model;

                    int total = 0;

                    var query = from promo in _db.Promos.Where(w => !w.IsDeleted)
                                join pGroup in _db.PromoGroups.Where(w => !w.IsDeleted)
                                on promo.GroupId equals pGroup.Id into pG
                                from x in pG.DefaultIfEmpty()
                                select new PromoModel
                                {
                                    Id = promo.Id,
                                    Name = promo.Name,
                                    EndTime = promo.EndTime,
                                    IsActive = !promo.EndTime.HasValue || promo.EndTime.Value > DateTime.Now,
                                    IsDeleted = false,
                                    MaxCount = promo.MaxCount,
                                    MaxOrderAmount = promo.MaxOrderAmount,
                                    MinOrderAmount = promo.MinOrderAmount,
                                    OrderDiscount = promo.OrderDiscount,
                                    StartTime = promo.StartTime,
                                    TotalCount = promo.TotalCount,
                                    Type = promo.Type,
                                    View = promo.View,
                                    GroupName = x != null ? x.Name : "",
                                    groupId = x != null ? x.Id : 0,
                                };

                    if (!string.IsNullOrEmpty(model.SearchText))
                        query = query.Where(w => (!string.IsNullOrEmpty(w.GroupName) &&
                        w.GroupName.Contains(model.SearchText)) ||
                        w.Name.Contains(model.SearchText));

                    if (!model.IsArchive)
                        query = query.Where(w => w.IsActive);

                    total = query.Count();

                    query = query.OrderByDescending(o => o.StartTime);

                    result.List = query.Skip(model.Skip).Take(model.Take).ToList();

                    result.Total = total;

                    return new ApiBaseResultModel<ListBaseModel<PromoModel>>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<ListBaseModel<PromoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
