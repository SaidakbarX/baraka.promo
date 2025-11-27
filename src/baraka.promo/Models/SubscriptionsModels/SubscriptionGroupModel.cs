namespace baraka.promo.Models.SubscriptionsModels
{
    public class SubscriptionGroupModel
    {
        public long Id { get; set; }
        public int CountryId { get; set; }       
        public DateTime? ValidityPeriod { get; set; }
        public DisplayModel DisplayInfo { get; set; }
        public DisplayModel DisplayPurchaseInfo { get; set; }
    }
}
