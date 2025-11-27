
using baraka.promo.Models.LoyaltyApiModels.FilterModels;

namespace baraka.promo.Models.PushModels
{
    public class NotificationModel
    {
        public Guid Id { get; set; }
        public string? MessageUz { get; set; }
        public string? MessageRu { get; set; }
        public string? MessageEn { get; set; }
        public string? MessageKz { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? ImagePath { get; set; }
        public DateTime? ReadAt { get; set; }

        public static string CACHE_KEY = "NotificationModel-KEY";
    }

    public class NotificationRequestModel
    {
        public string DeviceId { get; set; }
        public PageFilterModel? Filter { get; set; }
    }

    public class NotificationPatchModel
    {
        public string DeviceId { get; set; }
        public Guid MessageId { get; set; }
    }
}
