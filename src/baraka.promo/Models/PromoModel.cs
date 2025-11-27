using baraka.promo.Data;

namespace baraka.promo.Models
{
    public class PromoModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int? MaxCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public long? MinOrderAmount { get; set; }
        public long? MaxOrderAmount { get; set; }
        public int? OrderDiscount { get; set; }
        public PromoView View { get; set; }
        public PromoType Type { get; set; }
        public int? TotalCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int TotalUsedCount { get; set; }

        public int? groupId { get; set; }
        public string? GroupName { get; set; }
        public bool IsUnique { get; set; }
        public bool IsPromotion { get; set; }


        public static string CACHE_KEY = "PromoModel-KEY";
    }
}
