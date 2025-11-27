
namespace baraka.promo.Models.SubscriptionsModels
{
    public class SubscriptionFrontModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ProductId { get; set; }
        public int MaxCount { get; set; }
        public int ClientsCount { get; set; }
    }
}
