using baraka.promo.Core.Cardholders;
using baraka.promo.Data;
using baraka.promo.Models.LoyaltyApiModels.Cardholders;
using baraka.promo.Models.LoyaltyApiModels.Cards;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace baraka.promo.Core.PromoMethods.PromoV2
{
    public class GetPromoList
    {
        public class Command : IRequest<ApiBaseResultModel<ListBaseModel<PromoModel>>>
        {
            public Command(PromoFilter filter)
            {
                Filter = filter;
            }
            public PromoFilter Filter { get; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<ListBaseModel<PromoModel>>>
        {
            readonly ILogger<GetCardholders> _logger;
            readonly Func<ApplicationDbContext> _dbFactory;
            readonly ICurrentUser _current_user;
            readonly IMemoryCache _memory_cache;

            public Handler(
                ILogger<GetCardholders> logger,
                Func<ApplicationDbContext> dbFactory,
                ICurrentUser currentUser,
                IMemoryCache memoryCache)
            {
                _logger = logger;
                _dbFactory = dbFactory;
                _current_user = currentUser;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<ListBaseModel<PromoModel>>> Handle(
            Command request,
             CancellationToken cancellationToken)
            {
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null)
                        return new ApiBaseResultModel<ListBaseModel<PromoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));

                    if (!_current_user.IsAdmin())
                        return new ApiBaseResultModel<ListBaseModel<PromoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));

                    var filter = request.Filter;

                    using var db = _dbFactory(); 

                    var baseQuery = db.Promos
                        .AsNoTracking()
                        .Where(w => !w.IsDeleted);

                    if (filter.IsArchive)
                        baseQuery = baseQuery.Where(w => (!w.IsActive) || (!w.EndTime.HasValue || w.EndTime < DateTime.Now));
                    else
                        baseQuery = baseQuery.Where(w => (w.IsActive) && (!w.EndTime.HasValue || w.EndTime >= DateTime.Now));

                    if (!string.IsNullOrEmpty(filter.SearchText))
                        baseQuery = baseQuery.Where(w => w.Name.Contains(filter.SearchText));

                    if (filter.promoAudtoria.Count > 0)
                    {
                        var enumPromoAuditoria = EnumHelper<PromoType>.ListToEnumList(filter.promoAudtoria);
                        baseQuery = baseQuery.Where(w => enumPromoAuditoria.Contains(w.Type));
                    }

                    if (filter.machanicTypes.Count > 0)
                    {
                        var enumMachanicTypes = EnumHelper<PromoView>.ListToEnumList(filter.machanicTypes);
                        baseQuery = baseQuery.Where(w => enumMachanicTypes.Contains(w.View));
                    }

                    var countQuery = baseQuery.AsQueryable();
                    int total = await countQuery.CountAsync(cancellationToken);

                    var listQuery = baseQuery
                        .OrderByDescending(w => w.StartTime)
                        .Skip(filter.Skip)
                        .Take(filter.Take)
                        .Select(p => new PromoModel
                        {
                            Id = p.Id,
                            Name = p.Name,
                            StartTime = p.StartTime,
                            EndTime = p.EndTime,
                            groupId = p.GroupId,
                            GroupName = "",
                            IsActive = (p.IsActive) && (!p.EndTime.HasValue || p.EndTime >= DateTime.Now),
                            IsDeleted = p.IsDeleted,
                            MaxCount = p.MaxCount,
                            MaxOrderAmount = p.MaxOrderAmount,
                            MinOrderAmount = p.MinOrderAmount,
                            OrderDiscount = p.OrderDiscount,
                            TotalCount = p.TotalCount,
                            Type = p.Type,
                            View = p.View,
                            IsUnique = p.IsUnique,
                            IsPromotion = p.IsPromotion,
                        });

                    var result = await listQuery.ToListAsync(cancellationToken);

                    var ids = result.Select(r => r.Id).ToList();

                    var clients = await db.PromoClients
                        .AsNoTracking()
                        .Where(a => a.TimeOfUse.HasValue && ids.Contains(a.PromoId))
                        .GroupBy(g => g.PromoId)
                        .Select(s => new
                        {
                            PromoId = s.Key,
                            Count = s.Count()
                        })
                        .ToListAsync(cancellationToken);

                    foreach (var item in result)
                    {
                        var client = clients.FirstOrDefault(f => f.PromoId == item.Id);
                        if (client != null)
                            item.TotalUsedCount = client.Count;
                    }

                    return new ApiBaseResultModel<ListBaseModel<PromoModel>>(new ListBaseModel<PromoModel>
                    {
                        List = result,
                        Total = total
                    });
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