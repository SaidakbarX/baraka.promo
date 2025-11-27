
using baraka.promo.Core.Cards;
using baraka.promo.Data;
using baraka.promo.Models;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Models.PromoModels.NewPromoModels;
using baraka.promo.Services;
using baraka.promo.Utils;
using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace baraka.promo.Core.PromoMethods.PromoV2
{
    public class GetPromoListGroupped
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
            readonly Func<ApplicationDbContext> _dbFactory;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<AddCard> logger, Func<ApplicationDbContext> dbFactory, ICurrentUser currentUser)
            {
                _logger = logger;
                _dbFactory = dbFactory;
                _current_user = currentUser;
            }

            public async Task<ApiBaseResultModel<ListBaseModel<PromoGroupModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                var result = new ListBaseModel<PromoGroupModel>();

                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null)
                        return new ApiBaseResultModel<ListBaseModel<PromoGroupModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));

                    if (!_current_user.IsAdmin())
                        return new ApiBaseResultModel<ListBaseModel<PromoGroupModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));

                    var model = request.Model;

                    using var db = _dbFactory();

                    var groupList = await db.PromoGroups
                        .AsNoTracking()
                        .Where(w => !w.IsDeleted)
                        .OrderByDescending(o => o.ModifiedTime)
                        .ToListAsync(cancellationToken);

                    var promoQueryBuilder = db.Promos
                        .AsNoTracking()
                        .Where(w => !w.IsDeleted && w.IsPromotion == model.IsPromotion);

                    if (model.IsArchive)
                    {
                        promoQueryBuilder = promoQueryBuilder.Where(w => !w.IsActive || (w.EndTime.HasValue && w.EndTime < DateTime.Now));
                    }
                    else
                    {
                        promoQueryBuilder = promoQueryBuilder.Where(w => w.IsActive && (!w.EndTime.HasValue || w.EndTime >= DateTime.Now));
                    }

                    if (!string.IsNullOrEmpty(model.SearchText))
                        promoQueryBuilder = promoQueryBuilder.Where(w => w.Name.Contains(model.SearchText));

                    if (model.promoAudtoria?.Count > 0)
                    {
                        var enumPromoAuditoria = EnumHelper<PromoType>.ListToEnumList(model.promoAudtoria);
                        promoQueryBuilder = promoQueryBuilder.Where(w => enumPromoAuditoria.Contains(w.Type));
                    }

                    if (model.machanicTypes?.Count > 0)
                    {
                        var enumMachanicTypes = EnumHelper<PromoView>.ListToEnumList(model.machanicTypes);
                        promoQueryBuilder = promoQueryBuilder.Where(w => enumMachanicTypes.Contains(w.View));
                    }

                    var promoList = await promoQueryBuilder.ToListAsync(cancellationToken);

                    var promoClients = await db.PromoClients
                        .AsNoTracking()
                        .Where(a => a.TimeOfUse.HasValue)
                        .GroupBy(g => g.PromoId)
                        .Select(s => new
                        {
                            PromoId = s.Key,
                            Count = s.Count()
                        })
                        .ToListAsync(cancellationToken);


                    var groupIds = groupList.Select(s => s.Id).ToList();

                    var promoWithGroup = promoList.Where(w => w.GroupId != null && groupIds.Contains((int)w.GroupId)).ToList();
                    var promoWithoutGroup = promoList.Where(w => w.GroupId == null || w.GroupId == 0).ToList();

                    var promoForClientsIds = promoWithGroup.Select(s => s.Id).ToList();
                    var promoForNoGroupIds = promoWithoutGroup.Select(s => s.Id).ToList();

                    var promoClientsGrouped = promoClients.Where(w => promoForClientsIds.Contains(w.PromoId)).ToList();
                    var promoClientsNoGroup = promoClients.Where(w => promoForNoGroupIds.Contains(w.PromoId)).ToList();

                    int noGroupPromoCount = 0;
                    if (model.Skip == 0)
                    {
                        model.Take -= 1;
                        noGroupPromoCount = promoForNoGroupIds.Count;
                    }

                    var groupsPage = groupList
                        .Skip(model.Skip)
                        .Take(model.Take)
                        .ToList();

                    result.List = groupsPage.Select(s => new PromoGroupModel
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        Order = s.Order,
                        ModifiedBy = s.ModifiedBy
                    }).ToList();

                    foreach (var item in result.List)
                    {
                        var groupPromos = promoWithGroup.Where(w => w.GroupId == item.Id).ToList();
                        var groupPromoIds = groupPromos.Select(p => p.Id).ToList();

                        item.TotalActive = groupPromos.Count(w => w.IsActive && (!w.EndTime.HasValue || w.EndTime >= DateTime.Now));
                        item.TotalInActive = groupPromos.Count(w => !w.IsActive || (w.EndTime.HasValue && w.EndTime < DateTime.Now));
                        item.TotalUsedCount = promoClientsGrouped
                            .Where(s => groupPromoIds.Contains(s.PromoId))
                            .Sum(s => s.Count);

                        item.MemberCount = groupPromos.Count;
                    }

                    result.List = result.List.Where(w => w.MemberCount > 0).ToList();

                    if (noGroupPromoCount > 0 && model.Skip == 0)
                    {
                        var groupPromos = promoWithoutGroup;

                        result.List.Add(new PromoGroupModel
                        {
                            Id = 0,
                            Name = model.IsPromotion ? "Не сгруппированные акции" : "Не сгруппированные промо",
                            Description = model.IsPromotion ? "Акции без группы" : "Промоакции без группы",
                            Order = 0,
                            MemberCount = noGroupPromoCount,
                            ModifiedBy = "System",
                            TotalUsedCount = promoClientsNoGroup.Sum(s => s.Count),
                            TotalActive = groupPromos.Count(w => w.IsActive && (!w.EndTime.HasValue || w.EndTime >= DateTime.Now)),
                            TotalInActive = groupPromos.Count(w => !w.IsActive || (w.EndTime.HasValue && w.EndTime < DateTime.Now))
                        });
                    }

                    result.List = result.List
                        .OrderByDescending(o => o.MemberCount)
                        .ThenByDescending(o => o.Order)
                        .ToList();

                    result.Total = groupList.Count + (noGroupPromoCount > 0 ? 1 : 0);

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