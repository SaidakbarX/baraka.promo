using baraka.promo.Core.Transactions;
using baraka.promo.Models.LoyaltyApiModels.Transactions;
using baraka.promo.Models;
using baraka.promo.Services.HelperServices;
using MediatR;
using baraka.promo.Core.PromoMethods;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Models.PromoModels.NewPromoModels;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Core.PromoMethods.PromoV2;

namespace baraka.promo.Services.PromGroup
{
    public class PromoGroupService : BaseFrontService
    {
        public PromoGroupService(IMediator mediator, MyNotificationService notificationService) : base(mediator, notificationService)
        {
        }

        public async Task<ListBaseModel<PromoGroupModel>> List(int skip, int take, string search, List<string> machanicTypes, List<string> promoAudtoria, bool isArchive = true)
        {
            PromoFilter filter = new()
            {
                Skip = skip,
                Take = take,
                SearchText = search,
                IsArchive = isArchive,
                machanicTypes = machanicTypes,
                promoAudtoria = promoAudtoria
            };
            var command = new GetPromoGroupList.Command(filter);
            var result = await _mediator.Send(command);
            return Handler.Handle(new(), result);
        }
        

        //Gets list of promos wit groupped in table
        public async Task<ListBaseModel<PromoGroupModel>> GrouppedList(int skip, int take, string search, List<string> machanicTypes, List<string> promoAudtoria, bool isArchive = true, bool is_promotion = false)
        {
            PromoFilter filter = new()
            {
                Skip = skip,
                Take = take,
                SearchText = search,
                IsArchive = isArchive,
                machanicTypes = machanicTypes,
                promoAudtoria = promoAudtoria,
                IsPromotion = is_promotion,
            };
            var command = new GetPromoListGroupped.Command(filter);
            var result = await _mediator.Send(command);
            return Handler.Handle(new(), result);
        }


        public async Task<PromoGroupResult> AddUpdate(PromoGroupModel model)
        {

            var command = new AddUpdatePromoGroup.Command(model);
            var result = await _mediator.Send(command);
            return Handler.Handle(new(), result, true, successMessage: model.Id == 0 ? "Успешно добавлен" : "Успешно обновлено");
        }

        public async Task<bool> Delete(int id)
        {
            var command = new DeletePromoGroup.Command(id);
            var result = await _mediator.Send(command);
            return Handler.Handle(result, showError: true, showSuccess: true, successMessage: "Успешно удалено");
        }

    }
}
