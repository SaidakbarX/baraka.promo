using baraka.promo.Core.PromoMethods;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Models.PromoModels.NewPromoModels;
using baraka.promo.Models;
using baraka.promo.Services.HelperServices;
using MediatR;
using baraka.promo.Core.PromoMethods.PromoV2;
using baraka.promo.Core;

namespace baraka.promo.Services.Promo
{
    public class PromoService : BaseFrontService
    {
        public PromoService(IMediator mediator, MyNotificationService notificationService) : base(mediator, notificationService)
        {
        }

        public async Task<ListBaseModel<PromoModel>> List(int skip, int take, string search, bool IsArchive)
        {
            PromoFilter filter = new()
            {
                Skip = skip,
                Take = take,
                SearchText = search,
                IsArchive = IsArchive
            };
            var command = new GetPromosByGroupName.Command(filter);
            var result = await _mediator.Send(command);
            return Handler.Handle(new(), result);
        }
        
        public async Task<ListBaseModel<PromoModel>> GroupedList(int skip, int take, string search, List<string> machanicTypes, List<string> promoAudtoria, int? groupId, bool IsArchive = true, bool AllList = false)
        {
            PromoFilter filter = new()
            {
                Skip = skip,
                Take = take,
                SearchText = search,
                IsArchive = IsArchive,
                FromPromoList = AllList,
                machanicTypes = machanicTypes,
                promoAudtoria = promoAudtoria,
            };
            var command = new PromoGroupItems.Command(filter, groupId);
            var result = await _mediator.Send(command);
            return Handler.Handle(new(), result);
        }
        public async Task<ListBaseModel<PromoModel>> PromoList(int skip, int take, string search, List<string> machanicTypes, List<string> promoAudtoria, bool IsArchive = true)
        {
            PromoFilter filter = new()
            {
                Skip = skip,
                Take = take,
                SearchText = search,
                IsArchive = IsArchive,
                machanicTypes = machanicTypes,
                promoAudtoria = promoAudtoria,
            };
            var command = new GetPromoList.Command(filter);
            var result = await _mediator.Send(command);
            return Handler.Handle(new(), result);
        }

        public async Task<string> GetExcelFile(long promo_id)
        {
            var command = new GetPromoChildValuesToExcel.Command(promo_id);
            var result = await _mediator.Send(command);
            return result?.Data;
        }
    }
}
