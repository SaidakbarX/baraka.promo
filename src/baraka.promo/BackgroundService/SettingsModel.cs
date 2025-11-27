using baraka.promo.Data.Loyalty;
using baraka.promo.Data.Subscriptions;
using baraka.promo.Delivery;
using baraka.promo.Models.Enums;
using baraka.promo.Pages.TgPushSender;
using baraka.promo.Utils;

namespace baraka.promo.BackgroundService
{
    public class SettingsModel
    {
        public Guid MessageHeaderId { get; set; }
        public List<TgUserModel> ChatIds { get; set; }

    }
    public class DeviceSettingsModel
    {
        public Guid NotificationHeaderId { get; set; }
        public List<DeviceModel> Devices { get; set; }

    }

    public class DeviceModel
    {
        public string DeviceId { get; set; }
        public Language Language { get; set; }
    }

    public class PaymentSettingsModel
    {
        public Transaction transaction { get; set; }
        public Customer customer { get; set; }
        public long subscription_id { get; set; }
    }

    public class ClickPaymentSettingsModel
    {
        public Subscription subscription { get; set; }
        public string invoice_id { get; set; }
        public Transaction transaction { get; set; }
        public Customer customer { get; set; }
        public long subscription_id { get; set; }
    }
}
