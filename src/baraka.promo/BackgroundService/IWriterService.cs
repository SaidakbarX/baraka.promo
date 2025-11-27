

using baraka.promo.BackgroundService;

namespace Resident.Monitoring.Settings 
{ 
    public interface IWriterService
    {
        Task Monitor(SettingsModel settings);
        Task MonitorNotifications(DeviceSettingsModel settings);
        Task MonitorPayments(PaymentSettingsModel settings);
        Task MonitorClickPayments(ClickPaymentSettingsModel settings);
    }
}
