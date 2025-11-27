namespace baraka.promo.Models.OrderApiModel
{
    public class OrderModel
    {
        public string ClientPhone { get; set; }
        public List<OrderProductModel> Products { get; set; }
        public long OrderAmount { get; set; }
        public int RegionId { get; set; }
        public Guid RestaurantId { get; set; }
    }
    public class OrderProductModel
    {
        public Guid ProductId { get; set; }
        //public int Quantity { get; set; }
    }
}
