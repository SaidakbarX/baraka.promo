using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Resident.Monitoring.BackgroundModels;
using Serilog;
using SmsSender.Serivce.Baraka.Promo;
using SmsSender.Serivce.Services;


namespace SmsSender.Serivce
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
           .AddCommandLine(args)
           .AddJsonFile("appsettings.json")
           .Build();

            CreateHostBuilder(args, configuration).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfigurationRoot config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddSerilog(); // Add Serilog to the logging pipeline
                    logging.AddFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs/tg_push.txt")); // Add file logging
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<MainBackgroundTaskService>();
                    services.AddTransient<ISenderService, SenderService>();
                    services.AddSingleton<TgHelper>();
                    services.AddDbContext<PromoDbContext>(options =>
                        options.UseSqlServer(connectionString));
                }).UseWindowsService();
        }
    }
}