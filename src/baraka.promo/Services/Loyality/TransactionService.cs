using baraka.promo.Core.Cardholders;
using baraka.promo.Models.LoyaltyApiModels.Cardholders;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Models;
using baraka.promo.Services.HelperServices;
using MediatR;
using baraka.promo.Core.Transactions;
using baraka.promo.Models.LoyaltyApiModels.Transactions;
using baraka.promo.Models.LoyaltyApiModels;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using DocumentFormat.OpenXml.Bibliography;
using Telegram.Bot.Types;

namespace baraka.promo.Services.Loyality
{
    public class TransactionService : BaseFrontService
    {
        readonly ICurrentUser currentUser;
        public TransactionService(IMediator mediator, MyNotificationService notificationService, ICurrentUser currentUser) : base(mediator, notificationService)
        {
            this.currentUser = currentUser;
        }



        public string GetUser()
        {
            string user = "";
            try
            {
                user = currentUser.GetCurrentUserName();
            }
            catch (Exception e)
            {
                user = $"Error {e.Message}";
            }
            return user;
        }
        public async Task<ListBaseModel<TransactionInfoModel>> List(int skip, int take, string search)
        {
            TransactionRequestModel filter = new()
            {
                Skip = skip,
                Take = take,
                CardNumber = search,
            };
            var command = new GetTransactions.Command(filter);
            var result = await _mediator.Send(command);
            return Handler.Handle(new ListBaseModel<TransactionInfoModel>(), result);

        }
        public async Task<LoyaltyResultModel> Charge(string cardNumber, decimal amount, bool showSuccess = false)
        {
            string user = "";
            try
            {
                user = currentUser.GetCurrentUserName();
            }
            catch (Exception e)
            {
                user = $"Error {e.Message}";
            }
            TransactionModel model = new()
            {
                CardNumber = cardNumber,
                Sum = amount,
                ExternalId = Guid.NewGuid().ToString(),
                ExternalData = $"Charge_by_Service User: {user}",
            };
            var command = new AddTransaction.Command(model);
            var result = await _mediator.Send(command);
            return Handler.Handle(new LoyaltyResultModel(), result, showSuccess);
        }

        public async Task<LoyaltyResultModel> Replenish(string cardNumber, decimal amount, bool showSuccess = false)
        {
            ReplenishmentModel model = new()
            {
                CardNumber = cardNumber,
                Sum = amount
            };
            var command = new ReplenishBalance.Command(model);
            var result = await _mediator.Send(command);
            return Handler.Handle(new(), result, true);

        }

        public async Task<bool> Delete(Guid id)
        {
            var command = new DeleteTransaction.Command(id);
            var result = await _mediator.Send(command);
            return Handler.Handle(result, showError: true, showSuccess: true, successMessage: "Транзакция успешно отменена");
        }

    }

}
