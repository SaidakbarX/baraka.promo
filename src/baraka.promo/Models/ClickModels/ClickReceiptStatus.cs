namespace baraka.promo.Models.ClickModels
{
    public class ClickReceiptStatus
    {
        public int error_code { get; set; }
        public string error_note { get; set; }
        public int invoice_status { get; set; }
        public string invoice_status_note { get; set; }
        public long payment_id { get; set; }
    }
}
