using baraka.promo.Data;
using baraka.promo.Delivery;
using Newtonsoft.Json;
using System.Linq;

namespace baraka.promo.Core
{
    public class CheckSegment
    {
        private readonly ApplicationDbContext _db;
        private readonly DeliveryDbContext _ddb;
        public CheckSegment(DeliveryDbContext ddb, ApplicationDbContext db)
        {
            _ddb = ddb;
            _db = db;
        }

        public (bool isSuccess, string error) IsSegmentCompatible(string phone, int segmentId)
        {

            try
            {
                var segment = _db.Segments.Where(a => a.Id == segmentId).FirstOrDefault();
                if (segment == null)
                    return (false, "Сегмента не найден!");

                bool isDateFrom = (segment?.DateFrom) == null || DateTime.Now >= segment.DateFrom;
                if (!isDateFrom)
                    return (false, "Дата начало сегмента не настал!");

                bool isDateTo = (segment?.DateTo) == null || DateTime.Now <= segment.DateTo;
                if (!isDateTo)
                    return (false, "Дата использование сегмента истекла!");

                if (segment.IsNewClient)
                {
                    //var query = from c in _ddb.Customers
                    //            join o in _ddb.Orders on c.Id equals o.CustomerId into orr
                    //            from o in orr.DefaultIfEmpty()
                    //            where o == null && c.Phone1 == phone
                    //            select c;

                    var query = from d in _ddb.Deliveries
                                where d.Phone1 == phone
                                && d.Status != 250 && d.Status != 255
                                select d;

                    //var segmentUser = query.FirstOrDefault();
                    if (query.Count() > segment.NewClientOrdersCount)
                        return (false, "Клиент несовместим с критериями сегмента!");
                    else
                        return (true, null);
                }

                var queryTemp = from d in _ddb.Deliveries
                                join o in _ddb.Orders on d.OrderId equals o.Id
                                join i in _ddb.OrderItems on d.OrderId equals i.OrderId
                                join p in _ddb.Products on i.ProductId equals p.Id
                                where d.Status == 201
                                && d.Phone1 == phone
                                select new
                                {
                                    d.Phone1,
                                    Amount = i.Price * i.Quantity,
                                    o.DateTime,
                                    o.OrderTypeId,
                                    i.ProductId,
                                    p.CategoryId,
                                    p.SubCategoryId,
                                    o.RestaurantId,
                                    o.Id
                                };
                if (segment.OrderTimeFrom.HasValue)
                    queryTemp = queryTemp.Where(w => w.DateTime >= segment.OrderTimeFrom);
                if (segment.OrderTimeTo.HasValue)
                    queryTemp = queryTemp.Where(w => w.DateTime < segment.OrderTimeTo);

                if (segment.AmountMin.HasValue && segment.AmountMin > 0)
                {
                    queryTemp = queryTemp.Where(o => o.Amount >= segment.AmountMin);
                }

                if (segment.AmountMax.HasValue && segment.AmountMax > 0)
                {
                    queryTemp = queryTemp.Where(o => o.Amount <= segment.AmountMax);
                }

                if (!string.IsNullOrEmpty(segment.OrderTypeIds))
                {
                    var orderTypeIds = JsonConvert.DeserializeObject<List<int>>(segment.OrderTypeIds);
                    if (orderTypeIds?.Count() > 0)
                        queryTemp = queryTemp.Where(o => orderTypeIds.Contains(o.OrderTypeId));
                }

                if (!string.IsNullOrEmpty(segment.RestaurantIds))
                {
                    var restaurantIds = JsonConvert.DeserializeObject<List<Guid>>(segment.RestaurantIds);
                    if (restaurantIds?.Count() > 0)
                        queryTemp = queryTemp.Where(o => restaurantIds.Contains((Guid)o.RestaurantId));
                }

                var client = queryTemp.GroupBy(g => g.Phone1).Select(s => new
                {
                    TotalAmount = Math.Round(s.Sum(sa => sa.Amount), 0),
                    AvgAmount = Math.Round(s.Average(sa => sa.Amount), 0),
                    LastOrderTime = s.Max(m => m.DateTime),
                    FirstOrderTime = s.Min(m => m.DateTime),
                    OrderCount = s.Select(g => g.Id).Distinct().Count()
                }).FirstOrDefault();


                if (client == null)
                    return (false, "Клиент несовместим с критериями сегмента!");

                bool isOrderPeriodFrom = (segment?.OrderPeriodFrom) == null || DateTime.Now.AddDays(-(double)segment.OrderPeriodFrom) >= client.LastOrderTime;
                if (!isOrderPeriodFrom)
                    return (false, "Клиент несовместим с критериями сегмента!");

                bool isOrderPeriodTo = (segment?.OrderPeriodTo) == null || DateTime.Now.AddDays(-(double)segment.OrderPeriodTo) <= client.LastOrderTime;
                if (!isOrderPeriodTo)
                    return (false, "Клиент несовместим с критериями сегмента!");

                //var orderCount = _ddb.Deliveries.Where(a => a.Phone1 == phone && a.Status == 201).Select(a => a.OrderId).Count();

                bool isQuantityMin = (segment?.QuantityMin) == null || client.OrderCount >= segment.QuantityMin;
                if (!isQuantityMin)
                    return (false, "Клиент несовместим с критериями сегмента!");

                bool isQuantityMax = (segment?.QuantityMax) == null || client.OrderCount <= segment.QuantityMax;
                if (!isQuantityMax)
                    return (false, "Клиент несовместим с критериями сегмента!");

                bool isTotalAmountMin = (segment?.TotalAmountMin) == null || client.TotalAmount >= segment.TotalAmountMin;
                if (!isTotalAmountMin)
                    return (false, "Клиент несовместим с критериями сегмента!");

                bool isTotalAmountMax = (segment?.TotalAmountMax) == null || client.TotalAmount <= segment.TotalAmountMax;
                if (!isTotalAmountMax)
                    return (false, "Клиент несовместим с критериями сегмента!");

                bool isProductIds = true;
                if (!string.IsNullOrEmpty(segment?.ProductIds))
                {
                    var _segmentProducts = JsonConvert.DeserializeObject<List<string>>(segment.ProductIds);
                    if (_segmentProducts != null && _segmentProducts.Count > 0)
                    {
                        var segmentProducts = new List<Guid>();
                        foreach (var item in _segmentProducts.GroupBy(g => g))
                        {
                            if (Guid.TryParse(item.Key, out var productId))
                                segmentProducts.Add(productId);
                        }
                        if (segmentProducts.Any())
                        {

                            var productCount = queryTemp.Where(w => segmentProducts.Contains(w.ProductId))
                                .GroupBy(g => g.ProductId).Count();

                            if (segment.LogicalOperator)
                                isProductIds = productCount == segmentProducts.Count;
                            else
                                isProductIds = productCount > 0;


                            if (!isProductIds)
                                return (false, "Клиент несовместим с критериями сегмента!");
                        }
                    }
                }

                bool isCategoryIds = true;
                if (!string.IsNullOrEmpty(segment?.CategoryIds))
                {
                    var _segmentCategories = JsonConvert.DeserializeObject<List<string>>(segment.CategoryIds);
                    if (_segmentCategories != null && _segmentCategories.Count > 0)
                    {
                        var segmentCategories = new List<Guid>();
                        foreach (var item in _segmentCategories.GroupBy(g => g))
                        {
                            if (Guid.TryParse(item.Key, out var categoryId))
                                segmentCategories.Add(categoryId);
                        }
                        if (segmentCategories.Any())
                        {
                            var categoryCount = queryTemp.Where(w => segmentCategories.Contains(w.CategoryId))
                                .GroupBy(g => g.CategoryId).Count();

                            var subCategoryCount = queryTemp.Where(w => w.SubCategoryId.HasValue
                            && segmentCategories.Contains(w.SubCategoryId.Value))
                                .GroupBy(g => g.CategoryId).Count();

                            if (segment.LogicalOperator)
                                isCategoryIds = categoryCount <= subCategoryCount + segmentCategories.Count;
                            else
                                isProductIds = categoryCount > 0;

                            if (!isCategoryIds)
                                return (false, "Клиент несовместим с критериями сегмента!");
                        }
                    }
                }

                if (isDateFrom && isDateTo &&
                   isQuantityMin && isQuantityMax &&
                   isTotalAmountMin && isTotalAmountMax &&
                   isProductIds && isCategoryIds)
                {
                    return (true, null);
                }
                else
                    return (false, "Клиент несовместим с критериями сегмента!");
            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }
    }
}
