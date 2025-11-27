
namespace baraka.promo.Models.ClickModels
{
    public class ClickReceiptCreate
    {
        public int service_id { get; set; }
        public decimal amount { get; set; }
        public string phone_number { get; set; }
        public Guid merchant_trans_id { get; set; }
    }

    public class ClickReceiptResult
    {
        public int error_code { get; set; }
        public string error_note { get; set; }
        public long invoice_id { get; set; }
        public long payment_id { get; set; }
    }
}
