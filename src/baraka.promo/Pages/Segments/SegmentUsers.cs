using baraka.promo.Core;
using baraka.promo.Data;
using baraka.promo.Delivery;
using baraka.promo.Models;
using baraka.promo.Utils;
using MediatR;
using Newtonsoft.Json;

namespace baraka.promo.Pages.Segments
{
    public class SegmentUsers
    {
        readonly ApplicationDbContext _db;
        readonly DeliveryDbContext _ddb;
        readonly ILogger<SegmentUsers> _logger;
        public SegmentUsers(ApplicationDbContext db, DeliveryDbContext ddb, ILogger<SegmentUsers> logger)
        {
            _db = db;
            _ddb = ddb;
            _logger = logger;
        }
        public List<string> UserCount(int id)
        {
            try
            {
                var segment = _db.Segments.Where(a => a.Id == id).FirstOrDefault();

                if (segment != null)
                {
                    var queryTemp = from d in _ddb.Deliveries
                                    join o in _ddb.Orders on d.OrderId equals o.Id
                                    join i in _ddb.OrderItems on d.OrderId equals i.OrderId
                                    join p in _ddb.Products on i.ProductId equals p.Id
                                    where d.Status == 201
                                    select new
                                    {
                                        d.Phone1,
                                        Amount = o.SumOfItems + d.PriceOfDelivery,
                                        o.DateTime,
                                        o.OrderTypeId,
                                        i.ProductId,
                                        p.CategoryId,
                                        p.SubCategoryId,
                                        o.RestaurantId
                                    };

                    if (segment.OrderTimeFrom.HasValue)
                        queryTemp = queryTemp.Where(w => w.DateTime >= segment.OrderTimeFrom);
                    if (segment.OrderTimeTo.HasValue)
                        queryTemp = queryTemp.Where(w => w.DateTime <= segment.OrderTimeTo);

                    if (segment.AmountMin.HasValue)
                    {
                        queryTemp = queryTemp.Where(o => o.Amount >= segment.AmountMin);
                    }

                    if (segment.AmountMax.HasValue)
                    {
                        queryTemp = queryTemp.Where(o => o.Amount <= segment.AmountMax);
                    }

                    if (!string.IsNullOrEmpty(segment.OrderTypeIds))
                    {
                        var orderTypeIds = JsonConvert.DeserializeObject<List<int>>(segment.OrderTypeIds);
                        queryTemp = queryTemp.Where(o => orderTypeIds.Contains(o.OrderTypeId));
                    }

                    if (!string.IsNullOrEmpty(segment.RestaurantIds))
                    {
                        var restaurantIds = JsonConvert.DeserializeObject<List<Guid>>(segment.RestaurantIds);
                        queryTemp = queryTemp.Where(o => restaurantIds.Contains((Guid)o.RestaurantId));
                    }



                    var clients = queryTemp.GroupBy(g => g.Phone1).Select(s => new
                    {
                        Phone1 = s.Key,
                        TotalAmount = Math.Round(s.Sum(sa => sa.Amount), 0),
                        AvgAmount = Math.Round(s.Average(sa => sa.Amount), 0),
                        LastOrderTime = s.Max(m => m.DateTime),
                        FirstOrderTime = s.Min(m => m.DateTime),
                        OrderCount = s.GroupBy(g => g.DateTime).Count(),
                    });

                    if (segment.OrderPeriodFrom.HasValue)
                    {
                        clients = clients.Where(a => DateTime.Now.AddDays(-(double)segment.OrderPeriodFrom) >= a.LastOrderTime);
                    }

                    if (segment.OrderPeriodTo.HasValue)
                    {
                        clients = clients.Where(a => DateTime.Now.AddDays(-(double)segment.OrderPeriodTo) <= a.LastOrderTime);
                    }

                    if (segment.TotalAmountMin.HasValue)
                    {
                        clients = clients.Where(a => a.TotalAmount >= segment.TotalAmountMin);
                    }

                    if (segment.TotalAmountMax.HasValue)
                    {
                        clients = clients.Where(a => a.TotalAmount <= segment.TotalAmountMax);
                    }

                    var clientList = clients.Select(s => s.Phone1).ToList();

                    foreach (var item in clientList)
                    {
                        var orderCount = _ddb.Deliveries.Where(a => a.Phone1 == item && a.Status == 201).Select(b => b.OrderId).Count();

                        bool isQuantityMin = segment.QuantityMin.HasValue ? orderCount >= segment.QuantityMin : true;
                        bool isQuantityMax = segment.QuantityMax.HasValue ? orderCount <= segment.QuantityMax : true;

                        if (!isQuantityMin && !isQuantityMax)
                        {
                            clientList.Remove(item);
                        }
                    }

                    var clientProductItems = queryTemp
                        .Where(w => clientList.Contains(w.Phone1))
                        .GroupBy(g => new { g.Phone1, g.ProductId, g.CategoryId, g.SubCategoryId })
                        .Select(s =>
                            new
                            {
                                Phone = s.Key.Phone1,
                                ProductId = s.Key.ProductId,
                                CategoryId = s.Key.CategoryId,
                                SubCategoryId = s.Key.SubCategoryId,
                            }).ToList();

                    List<string> accessPhones = new List<string>();
                    if (!string.IsNullOrEmpty(segment?.ProductIds) && segment?.ProductIds != "[]")
                    {
                        List<Guid> _segmentProducts = JsonConvert.DeserializeObject<List<Guid>>(segment.ProductIds) ?? new List<Guid>();
                        int segmentCountProduct = _segmentProducts.Count;
                        clientProductItems = (from c in clientProductItems
                                              join sp in _segmentProducts on c.ProductId equals sp
                                              select c).ToList();
                        if (segment.LogicalOperator)
                            accessPhones = clientProductItems.GroupBy(x => x.Phone).Where(w => w.Count() >= segmentCountProduct).Select(s => s.Key).ToList();
                        else
                            accessPhones = clientProductItems.GroupBy(x => x.Phone).Select(s => s.Key).ToList();
                    }
                    else
                        accessPhones = clientProductItems.GroupBy(x => x.Phone).Select(s => s.Key).ToList();

                    if (!string.IsNullOrEmpty(segment?.CategoryIds) && segment?.CategoryIds != "[]")
                    {
                        List<Guid> _segmentCategories = JsonConvert.DeserializeObject<List<Guid>>(segment.CategoryIds) ?? new List<Guid>();
                        int segmentCountCategory = _segmentCategories.Count;
                        clientProductItems = (from c in clientProductItems
                                              join sp in _segmentCategories on c.CategoryId equals sp into spDef
                                              from sp in spDef.DefaultIfEmpty()
                                              join ssp in _segmentCategories on c.CategoryId equals ssp into sspDef
                                              from ssp in sspDef.DefaultIfEmpty()
                                              where sp != Guid.Empty && ssp != Guid.Empty && accessPhones.Contains(c.Phone)
                                              select c).ToList();

                        if (segment.LogicalOperator)
                            accessPhones = clientProductItems.GroupBy(x => x.Phone).Where(w => w.Count() >= segmentCountCategory).Select(s => s.Key).ToList();
                        else
                            accessPhones = clientProductItems.GroupBy(x => x.Phone).Select(s => s.Key).ToList();
                    }


                    clientList = clientList.Where(w => accessPhones.Contains(w)).ToList();
                    return new List<string>(clientList);

                }
                return new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPromoByName");
                return new List<string>();
            }
        }
    }
}
