namespace baraka.promo.Models.OrderApiModel
{
    public class PreorderResultModel
    {
        public string Status { get; set; }
        public OrderInfoModel Order { get; set; }
        public CustomerModel Customer { get; set; }
    }
}
