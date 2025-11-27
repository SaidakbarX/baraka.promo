namespace baraka.promo.Models.SubscriptionsModels
{
    public class SubscriptionApiModel
    {
        public List<SubscriptionGroupModel> groups { get; set; }
        public List<SubscriptionModel> subscriptions { get; set; }

        public static string CACHE_KEY = "SubscriptionsResultModel-KEY";
    }
}
