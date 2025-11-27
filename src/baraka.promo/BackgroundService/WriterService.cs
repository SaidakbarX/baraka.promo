using baraka.promo.BackgroundService;
using baraka.promo.Data;
using baraka.promo.Data.Subscriptions;
using baraka.promo.Models.Enums;
using baraka.promo.Models.PaymeModels;
using baraka.promo.Pages.TgPushSender;
using baraka.promo.Services;
using Microsoft.EntityFrameworkCore;
using Resident.Monitoring.Settings;
using System.Linq.Dynamic.Core;
using System.Security.Policy;
using Telegram.Bot.Types;

namespace baraka.promo
{
    public class WriterService : IWriterService
    {

        //private readonly ApplicationDbContext _db;
        private readonly IServiceScopeFactory _factoryService;
        private readonly ILogger<WriterService> _logger;
        public WriterService(
            ILogger<WriterService> logger,
            IServiceScopeFactory factoryService)
        {
            //_db = db;
            _factoryService = factoryService;
            _logger = logger;
        }

        public async Task Monitor(SettingsModel settings)
        {
            try
            {
                using var scope = _factoryService.CreateScope();
                var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var distinctChatIds = settings.ChatIds.DistinctBy(a => a.TelegramId).ToList();

                while (distinctChatIds.Count > 0)
                {
                    List<TgUserModel> tgUsers = new List<TgUserModel>();

                    tgUsers = distinctChatIds.Take(1000).ToList();

                    await _db.Messages.AddRangeAsync(tgUsers.Select(a => new Data.Message
                    {
                        ChatId = long.Parse(a.TelegramId),
                        MessageHeaderId = settings.MessageHeaderId,
                        Status = Models.TgMessageModel.Status.New,
                        Phone = a.Phone,
                    }).ToList());

                    distinctChatIds.RemoveRange(0, tgUsers.Count);

                    await _db.SaveChangesAsync();

                    await Task.Delay(1000); // wait 1 sec after write 100 user to database
                }

                var messageHeader = await _db.MessageHeaders.FirstOrDefaultAsync(a => a.Id == settings.MessageHeaderId);

                var status = _db.MessageHeaders.Any(a => a.Status == Models.MessageHeaderStatus.Start) ? Models.MessageHeaderStatus.Paused : Models.MessageHeaderStatus.Start;

                messageHeader.Status = status;
                await _db.SaveChangesAsync();

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return;
        }

        public async Task MonitorNotifications(DeviceSettingsModel settings)
        {
            try
            {
                using var scope = _factoryService.CreateScope();
                var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var distinctChatIds = settings.Devices.DistinctBy(a => a.DeviceId).ToList();

                while (distinctChatIds.Count > 0)
                {
                    List<DeviceModel> users = new List<DeviceModel>();

                    users = distinctChatIds.Take(1000).ToList();

                    await _db.Notifications.AddRangeAsync(users.Select(a => new Notification
                    {
                        DeviceId = a.DeviceId,
                        NotificationHeaderId = settings.NotificationHeaderId,
                        Status = Models.TgMessageModel.Status.New,
                        Language = a.Language,
                    }).ToList());

                    distinctChatIds.RemoveRange(0, users.Count);

                    await _db.SaveChangesAsync();

                    await Task.Delay(1000); // wait 1 sec after write 100 user to database
                }

                var push_header = await _db.NotificationHeaders.FirstOrDefaultAsync(a => a.Id == settings.NotificationHeaderId);

                var status = _db.NotificationHeaders.Any(a => a.Status == Models.MessageHeaderStatus.Start) ? Models.MessageHeaderStatus.Paused : Models.MessageHeaderStatus.Start;

                push_header.Status = status;
                await _db.SaveChangesAsync();

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return;
        }

        public async Task MonitorPayments(PaymentSettingsModel settings)
        {
            try
            {
                using var scope = _factoryService.CreateScope();
                var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _paymeService = scope.ServiceProvider.GetRequiredService<PaymeService>();
                var _mindBoxService = scope.ServiceProvider.GetRequiredService<MindBoxService>();

                DateTime dtExpire = DateTime.Now.AddMinutes(5);

                while (dtExpire > DateTime.Now)
                {
                    var status = await _paymeService.ReceiptCheck(settings.transaction.ExternalId);

                    if (status != null && status.Result.State == PaymeReceiptStatus.Four)
                    {
                        var is_mindbox = await _mindBoxService.CreateTransaction(settings.transaction.Id, settings.customer.Phone1,
                                                               settings.transaction.Sum, settings.transaction.ExternalData);

                        var transaction = await _db.Transactions.FirstOrDefaultAsync(x => x.Id == settings.transaction.Id);
                        transaction.SetStatus(TransactionStatus.Success);

                        SubscriptionClient subscriptionClient = new SubscriptionClient
                        {
                            Id = settings.customer.Id,
                            SubscriptionId = settings.subscription_id,
                        };
                        await _db.SubscriptionClients.AddAsync(subscriptionClient);
                        await _db.SaveChangesAsync();

                        break;
                    }

                    await Task.Delay(20000);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return;
        }

        public async Task MonitorClickPayments(ClickPaymentSettingsModel settings)
        {
            try
            {
                using var scope = _factoryService.CreateScope();
                var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _click_service = scope.ServiceProvider.GetRequiredService<ClickService>();
                var _mindBoxService = scope.ServiceProvider.GetRequiredService<MindBoxService>();

                DateTime dtExpire = DateTime.Now.AddMinutes(5);

                while (dtExpire > DateTime.Now)
                {
                    var status = await _click_service.ReceiptStatus(settings.invoice_id);

                    if (status != null && status.payment_id != 0)
                    {
                        var is_fiscalize = await _click_service.ReceiptFiscalize(settings.subscription, status.payment_id);

                        var is_mindbox = await _mindBoxService.CreateTransaction(settings.transaction.Id, settings.customer.Phone1,
                                                               settings.transaction.Sum, settings.transaction.ExternalData);

                        var transaction = await _db.Transactions.FirstOrDefaultAsync(x => x.Id == settings.transaction.Id);
                        transaction.SetStatus(TransactionStatus.Success);

                        SubscriptionClient subscriptionClient = new SubscriptionClient
                        {
                            Id = settings.customer.Id,
                            SubscriptionId = settings.subscription_id,
                        };
                        await _db.SubscriptionClients.AddAsync(subscriptionClient);
                        await _db.SaveChangesAsync();

                        break;
                    }

                    await Task.Delay(20000);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return;
        }
    }
}
