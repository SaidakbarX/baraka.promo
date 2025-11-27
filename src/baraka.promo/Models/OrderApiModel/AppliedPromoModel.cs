namespace baraka.promo.Models.OrderApiModel
{
    public class AppliedPromoModel
    {
        public long PromoId { get; set; }
        public string ClientPhone { get; set; }
        public string OrderId { get; set; }
        public long OrderAmount { get; set; }
        public int RegionId { get; set; }
        public Guid RestaurantId { get; set; }
        public List<Guid>? Products { get; set; }
    }
}
