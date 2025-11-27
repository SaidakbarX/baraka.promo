using baraka.promo.Core.Cardholders;
using baraka.promo.Data;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace baraka.promo.Core.PromoMethods
{
    public class PromoGroupItems
    {
        public class Command : IRequest<ApiBaseResultModel<ListBaseModel<PromoModel>>>
        {
            public Command(PromoFilter filter, int? groupId)
            {
                this.groupId = groupId;
                this.Filter = filter;
            }
            public int? groupId { get; set; }
            public PromoFilter Filter { get; private set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<ListBaseModel<PromoModel>>>
        {
            readonly ILogger<GetCardholders> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<GetCardholders> logger, ApplicationDbContext db, ICurrentUser currentUser, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<ListBaseModel<PromoModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null) return new ApiBaseResultModel<ListBaseModel<PromoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));
                    if (!_current_user.IsAdmin()) return new ApiBaseResultModel<ListBaseModel<PromoModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));

                    var cache_key = PromoModel.CACHE_KEY;
                    int total = 0;
                    var Model = request.Filter;
                    var grId = request.groupId;
                    List<PromoModel> result = new();
                    var query = _db.Promos.Where(w => !w.IsDeleted);

                    if (Model.IsArchive)
                        query = query.Where(w => (!w.IsActive) || (!w.EndTime.HasValue || w.EndTime < DateTime.Now));
                    else
                        query = query.Where(w => (w.IsActive) && (!w.EndTime.HasValue || w.EndTime >= DateTime.Now));


                    if (grId != null && grId > 0)
                        query = query.Where(w => w.GroupId == grId);
                    else
                        query = query.Where(w => w.GroupId == null || w.GroupId == 0);

                    if (!string.IsNullOrEmpty(Model.SearchText))
                        query = query.Where(w => w.Name.Contains(Model.SearchText));

                    query = query.OrderByDescending(w => w.StartTime);

                    var clientsQuery = _db.PromoClients.Where(a => a.TimeOfUse.HasValue).GroupBy(g => g.PromoId).Select(s => new
                    {
                        PromoId = s.Key,
                        Count = s.Count()
                    });

                    if (Model.promoAudtoria.Count > 0)
                    {
                        var enumPromoAuditoria = EnumHelper<PromoType>.ListToEnumList(Model.promoAudtoria);
                        query = query.Where(w => enumPromoAuditoria.Contains(w.Type));
                    }

                    if (Model.machanicTypes.Count > 0)
                    {
                        var enumMachanicTypes = EnumHelper<PromoView>.ListToEnumList(Model.machanicTypes);
                        query = query.Where(w => enumMachanicTypes.Contains(w.View));
                    }

                    total = query.Count();

                    result = (from p in query
                              //join c in clientsQuery on p.Id equals c.PromoId into cs
                              //from c in cs.DefaultIfEmpty()
                              select new PromoModel
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
                                  //TotalUsedCount = c != null ? c.Count : 0,
                                  Type = p.Type,
                                  View = p.View,
                                  IsUnique = p.IsUnique,
                                  IsPromotion = p.IsPromotion,
                              }).Skip(Model.Skip).Take(Model.Take).ToList();

                    List<long> ids = result.Select(s => s.Id).ToList();

                    var clients = clientsQuery.Where(w => ids.Contains(w.PromoId)).ToList();

                    foreach (var item in result)
                    {
                       var client = clients.FirstOrDefault(f => f.PromoId == item.Id);
                        if (client != null)
                            item.TotalUsedCount=client.Count;
                    }

                    //result.ForEach(f => f.TotalUsedCount = clients.Where(c => c.PromoId == f.Id).Sum());



                    return new ApiBaseResultModel<ListBaseModel<PromoModel>>(new ListBaseModel<PromoModel> { List = result, Total = total });
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
