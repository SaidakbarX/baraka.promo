namespace baraka.promo.Models.CardPaymentModels
{
    public class CardPaymentModel
    {
        public decimal amount { get; set; }
        public int store_id { get; set; }
        public Guid order_id { get; set; }
        public List<OfdModel> ofd { get; set; }
    }

    public class OfdModel
    {
        public string name { get; set; }
        public string mxik { get; set; }
        public string package_code { get; set; }
        public string tin { get; set; }
        public string unit { get; set; }
        public decimal price { get; set; }
        public decimal qty { get; set; }
        public int vat { get; set; }
        public decimal total { get; set; }
    }

    public class CardPaymentResponse
    {
        public string id { get; set; }
    }
}
