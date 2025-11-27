
using baraka.promo.BackgroundService;

namespace baraka.promo.BackgroundService.BackgroundModels;

public class TaskSettings
{
    public SettingsModel? SettingsModel { get; set; }

    public DeviceSettingsModel? DeviceSettingsModel { get; set; }

    public PaymentSettingsModel? PaymentSettingsModel { get; set; }

    public ClickPaymentSettingsModel? ClickPaymentSettingsModel { get; set; }

    public static TaskSettings FromMonitorSettings(SettingsModel settingsModel)
    {
        return new TaskSettings { SettingsModel = settingsModel };
    }

    public static TaskSettings FromDeviceMonitorSettings(DeviceSettingsModel settingsModel)
    {
        return new TaskSettings { DeviceSettingsModel = settingsModel };
    }

    public static TaskSettings FromPaymentMonitorSettings(PaymentSettingsModel settingsModel)
    {
        return new TaskSettings { PaymentSettingsModel = settingsModel };
    }

    public static TaskSettings FromClickPaymentMonitorSettings(ClickPaymentSettingsModel settingsModel)
    {
        return new TaskSettings { ClickPaymentSettingsModel = settingsModel };
    }
}