using Castle.Components.DictionaryAdapter;
using System.ComponentModel.DataAnnotations.Schema;

namespace baraka.promo.Data
{
    public class Segment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? OrderTimeFrom { get; set; }
        public DateTime? OrderTimeTo { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? OrderPeriodFrom { get; set; }
        public int? OrderPeriodTo { get; set; }
        public int? QuantityMin { get; set; }
        public int? QuantityMax { get; set; }
        public long? AmountMin { get; set; }    
        public long? AmountMax { get; set; }
        public long? TotalAmountMin { get; set; }
        public long? TotalAmountMax { get; set; }
        public string? OrderTypeIds { get; set; }
        public string? CategoryIds { get; set; }
        public string? ProductIds { get; set; }
        public bool LogicalOperator { get; set; }
        public string? RestaurantIds { get; set; }
        public bool IsNewClient { get; set; }
        public int NewClientOrdersCount { get; set; }
        public bool IsDeleted { get; set; }
    }

}