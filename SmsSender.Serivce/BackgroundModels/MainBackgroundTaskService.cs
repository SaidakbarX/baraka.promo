


using SmsSender.Serivce.Services;

namespace Resident.Monitoring.BackgroundModels
{
    public class MainBackgroundTaskService : Microsoft.Extensions.Hosting.BackgroundService
    {

        private readonly ISenderService _senderService;

        private readonly TimeSpan _timeSpan = TimeSpan.FromSeconds(10);//get number of seconds from config

        public MainBackgroundTaskService(ISenderService senderService)
        {
            _senderService = senderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new(_timeSpan);
            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                await _senderService.SendData();
            }
        }
    }
}
