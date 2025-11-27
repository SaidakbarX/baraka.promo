using baraka.promo.Data;
using baraka.promo.Models.Enums;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using baraka.promo.Models.SubscriptionsModels;
using baraka.promo.Delivery;
using Microsoft.EntityFrameworkCore;
using baraka.promo.Data.Loyalty;
using Microsoft.Extensions.Options;
using baraka.promo.BackgroundService.BackgroundModels;
using baraka.promo.Models.CardPaymentModels;
using DocumentFormat.OpenXml.Wordprocessing;
using baraka.promo.Data.Subscriptions;
using Newtonsoft.Json;

namespace baraka.promo.Core.Subscriptions
{
    public class AddSubscriptionCheck
    {
        public class Command : IRequest<ApiBaseResultModel>
        {
            public Command(SubscriptionCheckModel model, string user)
            {
                Model = model ?? throw new ArgumentNullException(nameof(model));
                User = user;
            }

            public SubscriptionCheckModel Model { get; set; }
            public string User { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel>
        {
            readonly ILogger<AddSubscriptionCheck> _logger;
            readonly ApplicationDbContext _db;
            readonly PaymeService _paymeService;
            readonly ClickService _clickService;
            readonly DeliveryDbContext _deliveryDbContext;
            readonly MindBoxService _mindBoxService;
            readonly TasksToRun _tasksToRun;
            readonly CardPaymentService _cardPaymentService;

            public Handler(ILogger<AddSubscriptionCheck> logger, ApplicationDbContext db, 
                PaymeService paymeService, ClickService clickService, DeliveryDbContext deliveryDbContext,
                MindBoxService mindBoxService, TasksToRun tasksToRun, CardPaymentService cardPaymentService)
            {
                _logger = logger;
                _db = db;
                _paymeService = paymeService;
                _clickService = clickService;
                _deliveryDbContext = deliveryDbContext;
                _mindBoxService = mindBoxService;
                _tasksToRun = tasksToRun;
                _cardPaymentService = cardPaymentService;
            }

            public async Task<ApiBaseResultModel> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var model = request.Model;

                    _logger.LogWarning($"AddSubscriptionCheck -> {JsonConvert.SerializeObject(model)}");

                    var subscription = await _db.Subscriptions.FindAsync(model.SubscriptionId);

                    if(subscription == null) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_NOT_FOUND));

                    var customer = await _deliveryDbContext.Customers.FirstOrDefaultAsync(x=>x.Id == model.CustomerId);

                    if (customer == null) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_USER_NOT_FOUND));

                    Transaction transaction = new Transaction(Guid.Empty, "", 0, subscription.Price, TransactionType.Invoice, request.User,
                                                           TransactionStatus.Draft, null, subscription.ProductId, null);

                    await _db.Transactions.AddAsync(transaction, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);

                    if (model.PaymentType == SubscriptionCheckType.Payme)
                    {
                        var receipt_id = await _paymeService.ReceiptCreate(subscription, transaction.Id);
                        if(receipt_id == null) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));

                        var receipt_send = await _paymeService.ReceiptSend(receipt_id, customer.Phone1);

                        if(!receipt_send) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));

                        transaction.SetExternalId(receipt_id);
                        await _db.SaveChangesAsync(cancellationToken);

                        _tasksToRun.Enqueue(TaskSettings.FromPaymentMonitorSettings(new BackgroundService.PaymentSettingsModel
                        {
                            transaction = transaction,
                            customer = customer,
                            subscription_id = subscription.Id,
                        }));
                    }
                    else if (model.PaymentType == SubscriptionCheckType.Click)
                    {
                        var receipt_id = await _clickService.ReceiptCreate(subscription, transaction.Id, customer.Phone1);
                        if (receipt_id == null || receipt_id == "0") return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));

                        transaction.SetExternalId(receipt_id);
                        await _db.SaveChangesAsync(cancellationToken);

                        _tasksToRun.Enqueue(TaskSettings.FromClickPaymentMonitorSettings(new BackgroundService.ClickPaymentSettingsModel
                        {
                            transaction = transaction,
                            customer = customer,
                            subscription_id = subscription.Id,
                            invoice_id = receipt_id,
                            subscription = subscription,
                        }));
                    }
                    else if (model.PaymentType == SubscriptionCheckType.Card)
                    {
                        PaymentTokenModel paymentToken = new PaymentTokenModel { customer_id = model.CustomerId, card_id = model.CardId };
                        
                        var transaction_id = await _cardPaymentService.CreatePayment(paymentToken, subscription, transaction.Id);
                        if (transaction_id == null) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));

                        transaction.SetExternalId(transaction_id);
                        transaction.SetStatus(TransactionStatus.Success);

                        var is_mindbox = await _mindBoxService.CreateTransaction(transaction.Id, customer.Phone1, subscription.Price, subscription.ProductId);

                        SubscriptionClient subscriptionClient = new()
                        {
                            Id = customer.Id,
                            SubscriptionId = subscription.Id,
                        };

                        await _db.SubscriptionClients.AddAsync(subscriptionClient, cancellationToken);
                        await _db.SaveChangesAsync(cancellationToken);
                    }
                    else return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_INCORRECT_PARAM));

                    return new ApiBaseResultModel();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }        
    }
}
