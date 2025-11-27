namespace baraka.promo.Models.SubscriptionsModels
{
    public class SubscriptionCheckModel
    {
        public Guid CustomerId { get; set; }
        public long SubscriptionId { get; set; }
        public SubscriptionCheckType PaymentType { get; set; }
        public long CardId { get; set; }
    }

    public class SubscriptionCheckResult
    {
        public string redirect_url { get; set; }
    }

    public enum SubscriptionCheckType
    {
        Card = 4,
        Payme = 1,
        Click = 2
    }
}
