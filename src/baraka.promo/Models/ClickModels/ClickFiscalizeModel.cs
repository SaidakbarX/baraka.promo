namespace baraka.promo.Models.ClickModels
{
    public class ClickFiscalizeModel
    {
        public int service_id { get; set; }
        public long payment_id { get; set; }
        public decimal received_ecash { get; set; }
        public decimal received_cash { get; set; }
        public decimal received_card { get; set; }
        public List<ClickItemModel> items { get; set; }
    }

    public class ClickItemModel
    {
        public string Name { get; set; }
        public string SPIC { get; set; }
        public string PackageCode { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal VAT { get; set; }
        public int VATPercent { get; set; }
        public ClickCommissionInfo CommissionInfo { get; set; }
    }

    public class ClickCommissionInfo
    {
        public string TIN { get; set; }
    }
}
