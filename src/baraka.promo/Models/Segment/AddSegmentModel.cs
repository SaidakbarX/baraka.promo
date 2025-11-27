using System.ComponentModel.DataAnnotations.Schema;

namespace baraka.promo.Models.Segment
{
    public class AddSegmentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? OrderTimeFrom { get; set; }
        public DateTime? OrderTimeTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? OrderPeriodFrom { get; set; } = null;
        public int? OrderPeriodTo { get; set; } = null;
        public int? QuantityMin { get; set; } = null;
        public int? QuantityMax { get; set; } = null;
        public long? AmountMin { get; set; } = null;
        public long? AmountMax { get; set; } = null;
        public long? TotalAmountMin { get; set; } = null;
        public long? TotalAmountMax { get; set; } = null;
        public string? OrderTypeId { get; set; } = null;
        public string? OrderType { get; set; }
        public string? CategoryIds { get; set; }
        public string? ProductIds { get; set; }
        public bool LogicalOperator { get; set; }
        public string? RestaurantIds { get; set; }
        public bool IsNewClient { get; set; }
        public int NewClientOrdersCount { get; set; }
        public int SegmentUserCount { get; set; }
    }
}
