
namespace baraka.promo.Models.SubscriptionsModels
{
    public class SubscriptionModel
    {
        public long Id { get; set; }
        public int MaxCount { get; set; }
        public long GroupId { get; set; }
        public int CountryId { get; set; }
        public string ProductId { get; set; }
        public decimal Price { get; set; }
        public List<string> PromotionId { get; set; }
        public DisplayModel DisplayInfo { get; set; }
        public DisplayModel DisplayPurchaseInfo { get; set; }
    }

    public class DisplayModel
    {
        public string? NameUz { get; set; }
        public string? NameEn { get; set; }
        public string? NameRu { get; set; }
        public string? ShortContent_Ru { get; set; }
        public string? ShortContent_Uz { get; set; }
        public string? ShortContent_En { get; set; }
        public string? Content_Ru { get; set; }
        public string? Content_Uz { get; set; }
        public string? Content_En { get; set; }
        public string? ImageUrlRu { get; set; }
        public string? ImageUrlUz { get; set; }
        public string? ImageUrlEn { get; set; }
    }
}
