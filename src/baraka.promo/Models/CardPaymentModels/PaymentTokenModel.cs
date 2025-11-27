namespace baraka.promo.Models.CardPaymentModels
{
    public class PaymentTokenModel
    {
        public Guid customer_id { get; set; }
        public long card_id { get; set; }
    }
}
