using baraka.promo.Data;
using baraka.promo.Delivery;
using baraka.promo.Models;
using MediatR;

namespace baraka.promo.Core
{
    public class GetSegments
    {
        public class Command : IRequest<ApiBaseResultModel<List<Result>>>
        {
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<List<Result>>>
        {
            readonly ApplicationDbContext _db;
            readonly DeliveryDbContext _ddb;
            readonly ILogger<GetSegments> _logger;
            readonly CheckSegment _segment;
            public Handler(ApplicationDbContext db, DeliveryDbContext ddb, ILogger<GetSegments> logger, CheckSegment segment)
            {
                _db = db;
                _ddb = ddb;
                _logger = logger;
                _segment = segment;
            }
            public async Task<ApiBaseResultModel<List<Result>>> Handle(Command request, CancellationToken cancellationToken)
            {
                ApiBaseResultModel<List<Result>> result = new ApiBaseResultModel<List<Result>>();
                DateTime dtNow = DateTime.Now;
                result.Data = _db.Segments.Where(w=>w.IsDeleted == false && (w.DateFrom == null || w.DateFrom <= dtNow) && (w.DateTo == null || w.DateTo >= dtNow)).Select(s=> new Result
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();

                return result;
            }
        }

        public class Result
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
