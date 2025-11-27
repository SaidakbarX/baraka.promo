namespace baraka.promo.Models
{
    public class ExportModel
    {
        public string PhoneNumber { get; set; }
        public DateTime? FirstOrderTime { get; set; }
        public DateTime? LastOrderTime { get; set; }
        public int? FromLastOrderToNow { get; set; }
        public int? OrdersCount { get; set; }
        public decimal? OrderAmountCount { get; set; }
        public decimal? TotalOrdersAmount { get; set; }
        public string? ContactName { get; set; }
    }
}
