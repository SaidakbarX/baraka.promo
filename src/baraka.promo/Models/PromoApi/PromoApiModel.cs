using baraka.promo.Data;

namespace baraka.promo.Models.PromoApi
{
    public class PromoApiModel
    {
        public int? MaxCountForClient { get; set; }
        public string Phone { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxOrderAmount { get; set; }
        public int? OrderDiscount { get; set; }
        public int View { get; set; }
        public int? TotalCount { get; set; }
        public List<int>? Regions { get; set; }
        public List<Guid>? Restaurants { get; set; }
        public List<Guid>? ArbitrationProducts { get; set; }
        public List<PromoProductModel>? DiscountProducts { get; set; }
        public List<PromoProductModel>? PromotionalProducts { get; set; }
    }

    public class PromoResultModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
    }

    public class PromoProductModel
    {
        public string Id { get; set; }
        public int Count { get; set; }
        public int? Discount { get; set; }
    }
}