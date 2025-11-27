

using baraka.promo.BackgroundService;
using baraka.promo.BackgroundService.BackgroundModels;
using Microsoft.EntityFrameworkCore;
using Resident.Monitoring.Settings;

namespace Resident.Monitoring.BackgroundModels
{
    public class MainBackgroundTaskService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly TasksToRun _tasks;
        private readonly IWriterService _writerService;
        private readonly IServiceScopeFactory _factoryService;

        private readonly TimeSpan _timeSpan = TimeSpan.FromSeconds(10);//get number of seconds from config
        public MainBackgroundTaskService(TasksToRun tasks,
            IWriterService monitorService,
            IServiceScopeFactory factoryService)
        {
            _tasks = tasks;
            _writerService = monitorService;
            _factoryService = factoryService;          
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new(_timeSpan);
            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                var taskToRun = _tasks.Dequeue();
                if (taskToRun == null)
                {
                    //using var scope = _factoryService.CreateScope();
                    //var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    //var brand = _db.Brands.ToList().LastOrDefault(a => a.RequestState == State.Started);
                    //if (brand != null)
                    //{
                    //    _tasks.Enqueue(TaskSettings.FromMonitorSettings(new SettingsModel
                    //    {
                    //        BrandID = brand.Id,
                    //    }));
                    //}

                    continue;
                }
                //call the relevant service based on what 
                // settings object in not NULL

                if (taskToRun?.SettingsModel != null)
                {
                    await ExecuteMonitorTask(taskToRun.SettingsModel);
                }
                if (taskToRun?.DeviceSettingsModel != null)
                {
                    await ExecuteNotificationsMonitorTask(taskToRun.DeviceSettingsModel);
                }
                if (taskToRun?.PaymentSettingsModel != null)
                {
                    await ExecutePaymentsMonitorTask(taskToRun.PaymentSettingsModel);
                }
                if (taskToRun?.ClickPaymentSettingsModel != null)
                {
                    await ExecuteClickPaymentsMonitorTask(taskToRun.ClickPaymentSettingsModel);
                }
            }
        }

        public Task ExecuteMonitorTask(SettingsModel monitorSettings) =>
            _writerService.Monitor(monitorSettings);
        

        public Task ExecuteNotificationsMonitorTask(DeviceSettingsModel monitorSettings) =>
            _writerService.MonitorNotifications(monitorSettings);

        public Task ExecutePaymentsMonitorTask(PaymentSettingsModel monitorSettings) =>
            _writerService.MonitorPayments(monitorSettings);

        public Task ExecuteClickPaymentsMonitorTask(ClickPaymentSettingsModel monitorSettings) =>
            _writerService.MonitorClickPayments(monitorSettings);

    }
}
