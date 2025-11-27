using baraka.promo.Data;

namespace baraka.promo.Models.OrderApiModel
{
    public class OrderApiResultModel
    {
        public List<PromoApiResultModel>? Promo { get; set; }
    }

    public class PromoApiResultModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int? MaxCount { get; set; }
        public int? OrderDiscount { get; set; }
        public long? MinOrderAmount { get; set; }
        public long? MaxOrderAmount { get; set; }
        public PromoType PromoType { get; set; }
        public PromoView PromoView { get; set; }
        public List<DiscountProductModel>? ProductDiscount { get; set; }
        public List<FreeProductModel>? FreeProduct { get; set; }
        public PromotionalProductModel? PromotionalProduct { get; set; }
        public List<Guid>? ArbitrationProducts { get; set; }
    }

    public class FreeProductModel
    {
        public string ProductId { get; set; }
        public int Count { get; set; }
    }

    public class DiscountProductModel
    {
        public string ProductId { get; set; }
        public int Discount { get; set; }
        public int Count { get; set; }
    }
    public class PromotionalProductModel
    {
        public List<FreeProductModel> ProductIds { get; set; }
        public List<FreeProductModel> FreeProducts { get; set; }
    }
}
