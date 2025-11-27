using baraka.promo.Data;
using baraka.promo.Delivery;
using baraka.promo.Models.Paging;
using baraka.promo.Models.Segment;
using baraka.promo.Models;
using baraka.promo.Utils;
using MediatR;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace baraka.promo.Core
{
    public class GetExportData
    {
        public class Command : IRequest<ApiBaseResultModel<PageResultModel<ExportModel>>>
        {
            public Command(SegmentUserFilterModel model, bool withValues = true)
            {
                Filter = model;
                WithValues = withValues;
            }
            public SegmentUserFilterModel Filter { get; set; }
            public bool WithValues { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<PageResultModel<ExportModel>>>
        {
            readonly ApplicationDbContext _db;
            readonly DeliveryDbContext _ddb;
            readonly ILogger<GetSegmentUsers> _logger;
            readonly CheckSegment _segment;
            public Handler(ApplicationDbContext db, DeliveryDbContext ddb, ILogger<GetSegmentUsers> logger, CheckSegment segment)
            {
                _db = db;
                _ddb = ddb;
                _logger = logger;
                _segment = segment;
            }
            public async Task<ApiBaseResultModel<PageResultModel<ExportModel>>> Handle(Command request, CancellationToken cancellationToken)
            {

                try
                {
                    var model = request.Filter;
                    var id = model.Id;
                    var segment = _db.Segments.Where(a => a.Id == id).FirstOrDefault();


                    if (segment != null && !segment.IsNewClient)
                    {
                        bool isDateFrom = segment.DateFrom.HasValue ? segment?.DateFrom <= DateTime.Now : true;
                        bool isDateTo = segment.DateTo.HasValue ? segment?.DateTo > DateTime.Now : true;

                        if (isDateFrom == false || isDateTo == false)
                        {
                            return new ApiBaseResultModel<PageResultModel<ExportModel>>();
                        }

                        List<Guid> _segmentProducts = JsonConvert.DeserializeObject<List<Guid>>(segment.ProductIds ?? "[]") ?? new List<Guid>();
                        List<Guid> _segmentCategories = JsonConvert.DeserializeObject<List<Guid>>(segment.CategoryIds ?? "[]") ?? new List<Guid>();
                        int _segmentProductCount = _segmentProducts.Count;
                        int _segmentCategoryCount = _segmentCategories.Count;

                        var queryTemp = from d in _ddb.Deliveries
                                        join o in _ddb.Orders on d.OrderId equals o.Id
                                        join i in _ddb.OrderItems on d.OrderId equals i.OrderId
                                        join p in _ddb.Products on i.ProductId equals p.Id
                                        where
                                            d.Status == 201
                                            && (_segmentProductCount == 0 || _segmentProducts.Contains(p.Id))
                                            && (_segmentCategoryCount == 0
                                                || _segmentCategories.Contains(p.CategoryId)
                                                || (p.SubCategoryId != null && _segmentCategories.Contains(p.SubCategoryId.Value)))
                                        group i.Id by new { d.Phone1, d.ContactName, o.SumOfItems, o.DateTime, o.OrderTypeId, o.Id, o.RestaurantId } into gr
                                        select new
                                        {
                                            Phone1 = gr.Key.Phone1,
                                            ContactName = gr.Key.ContactName,
                                            Amount = gr.Key.SumOfItems,
                                            DateTime = gr.Key.DateTime,
                                            OrderTypeId = gr.Key.OrderTypeId,
                                            RestaurantId = gr.Key.RestaurantId,
                                            OrderId = gr.Key.Id,
                                            IsAccess = segment.LogicalOperator ? (gr.Count() >= _segmentProductCount) : true,
                                        };

                        queryTemp = queryTemp.Where(w => w.IsAccess);

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



                        var clients = queryTemp.GroupBy(g => new { g.Phone1 }).Select(s => new
                        {
                            Phone1 = s.Key.Phone1,
                            ContactName = s.FirstOrDefault().ContactName,
                            TotalAmount = Math.Round(s.Sum(sa => sa.Amount), 0),
                            AvgAmount = Math.Round(s.Average(sa => sa.Amount), 0),
                            LastOrderTime = s.Max(m => m.DateTime),
                            FirstOrderTime = s.Min(m => m.DateTime),
                            OrderCount = s.Select(g => g.OrderId).Distinct().Count(),
                        });

                        if (segment.OrderPeriodFrom.HasValue)
                        {
                            clients = clients.Where(a => DateTime.Now.AddDays(-(double)segment.OrderPeriodFrom) >= a.LastOrderTime);
                        }

                        if (segment.OrderPeriodTo.HasValue)
                        {
                            clients = clients.Where(a => DateTime.Now.AddDays(-(double)segment.OrderPeriodTo) <= a.LastOrderTime);
                        }

                        if (segment.QuantityMin.HasValue)
                        {
                            clients = clients.Where(a => a.OrderCount >= segment.QuantityMin);
                        }

                        if (segment.QuantityMax.HasValue && segment.QuantityMax > 0)
                        {
                            clients = clients.Where(a => a.OrderCount <= segment.QuantityMax);
                        }

                        if (segment.TotalAmountMin.HasValue)
                        {
                            clients = clients.Where(a => a.TotalAmount >= segment.TotalAmountMin);
                        }

                        if (segment.TotalAmountMax.HasValue)
                        {
                            clients = clients.Where(a => a.TotalAmount <= segment.TotalAmountMax);
                        }

                        int count = await clients.CountAsync(cancellationToken);
                        List<ExportModel> clientList = new List<ExportModel>();
                        if (request.WithValues)
                        {
                            clientList = await clients.Select(a => new ExportModel()
                            {
                                FirstOrderTime = a.FirstOrderTime,
                                LastOrderTime = (segment.OrderPeriodFrom.HasValue || segment.OrderPeriodTo.HasValue )? a.LastOrderTime : null,
                                TotalOrdersAmount = (segment.TotalAmountMin.HasValue || segment.TotalAmountMax.HasValue) ? a.TotalAmount : null,
                                OrdersCount = (segment.QuantityMin.HasValue || segment.QuantityMax.HasValue) ? a.OrderCount : null,
                                PhoneNumber = a.Phone1,
                                ContactName = a.ContactName,
                                OrderAmountCount = (segment.AmountMin.HasValue || segment.AmountMax.HasValue) ? a.OrderCount : null
                            }).ToListAsync(cancellationToken);

                            //if (request.Filter.Take > 0)
                            //{
                            //    clientList = clientList
                            //        .Skip(request.Filter.Skip)
                            //        .Take(request.Filter.Take).ToList();
                            //}

                        }


                        var resultList = new PageResultModel<ExportModel>
                        {
                            TotalCount = count,
                            Value = clientList
                        };

                        return new ApiBaseResultModel<PageResultModel<ExportModel>>(resultList);

                    }
                    return new ApiBaseResultModel<PageResultModel<ExportModel>>();
                }
                catch (TaskCanceledException)
                {
                    return new ApiBaseResultModel<PageResultModel<ExportModel>>(new PageResultModel<ExportModel>());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "GetExoprtData");
                    return new ApiBaseResultModel<PageResultModel<ExportModel>>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL, ex.Message));
                }
            }
        }
    }
}
