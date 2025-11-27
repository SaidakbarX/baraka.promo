using baraka.promo.Core.Cardholders;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models;
using baraka.promo.Models.LoyaltyApiModels.Cardholders;
using baraka.promo.Services.HelperServices;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using System.Text.RegularExpressions;
using baraka.promo.Models.Cards;
using baraka.promo.Core.Cards;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Data.Loyalty;

namespace baraka.promo.Services.Loyality
{
    public class LoyalityCardHolderService : BaseFrontService
    {

        public LoyalityCardHolderService(IMediator mediator, MyNotificationService notificationService) : base(mediator, notificationService)
        {

        }
        public async Task<ListBaseModel<CardholderInfoModel>> List(int skip, int take, string search)
        {
            FilterModel filter = new()
            {
                Skip = skip,
                Take = take,
                SearchText = search,
            };
            var command = new GetCardholders.Command(filter);
            var result = await _mediator.Send(command);
            return Handler.Handle(new ListBaseModel<CardholderInfoModel>(), result);
        }


        public async Task<LoyaltyResultModel> Add(CardholderModel model, string successMessage = "")
        {
            if (!string.IsNullOrEmpty(model.Phone))
                model.Phone = Regex.Replace(model.Phone, @"[+()\s-]", "");

            var command = new AddCardholder.Command(model);
            var result = await _mediator.Send(command);
            return Handler.Handle(new(), result, successMessage: successMessage);
        }


        public async Task<bool> Edit(Guid holderId, CardholderModel model, string successMessage = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(model.Phone))
                    model.Phone = Regex.Replace(model.Phone, @"[+()\s-]", "");

                var command = new EditCardholder.Command(holderId, model);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    notificationService.ErrorNotify(result.Error);
                else
                    notificationService.NotifySuccess("Данные клиента обновлена");
                return result.Success;
            }
            catch (Exception e)
            {
                notificationService.ErrorNotify(e);
                return false;
            }
        }

        public async Task<CardholderInfoModel> Get(Guid holder_id)
        {
            var command = new GetCardholder.Command(holder_id.ToString(), "", "", "");
            var result = await _mediator.Send(command);
            return Handler.Handle(new(), result);
        }
        public async Task<bool> Delete(Guid holder_id)
        {
            var command = new DeleteCardholder.Command(holder_id);
            var result = await _mediator.Send(command);
            return Handler.Handle(result, showError: true, showSuccess: true, successMessage: "Владелец карты успешно удален!");
        }



        ///Card Service
        public async Task<LoyaltyResultModel> AddCard(CardModel model)
        {
            var command = new AddCard.Command(model, "");
            var result = await _mediator.Send(command);
            return Handler.Handle(new(), result, successMessage: "Добавлено успешно");
        }
        public async Task<bool> EditCard(Guid cardId, CardModel model, string successMessage = "")
        {
            try
            {
                var command = new EditCard.Command(cardId, model);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    notificationService.ErrorNotify(result.Error);
                else
                    notificationService.NotifySuccess("Карта обновлена");

                return result.Success;
            }
            catch (Exception e)
            {
                notificationService.ErrorNotify(e);
                return false;
            }
        }
        public async Task<bool> CheckCardNumber(string number)
        {
            try
            {
                var command = new CheckNumber.Command(number);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    notificationService.ErrorNotify(result.Error);
                return result.Success;
            }
            catch (Exception e)
            {
                notificationService.ErrorNotify(e);
                return false;
            }
        }
        public async Task<bool> CheckPhoneNumber(string number)
        {
            try
            {
                var command = new CheckPhone.Command(number);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    notificationService.ErrorNotify(result.Error);
                return result.Success;
            }
            catch (Exception e)
            {
                notificationService.ErrorNotify(e);
                return false;
            }
        }

    }
}
