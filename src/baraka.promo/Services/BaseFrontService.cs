using baraka.promo.Services.HelperServices;
using MediatR;

namespace baraka.promo.Services
{
    public class BaseFrontService
    {

        public readonly IMediator _mediator;
        public readonly MyNotificationService notificationService;
        public readonly FrontResponseHandler Handler;
        public BaseFrontService(IMediator mediator, MyNotificationService notificationService)
        {
            _mediator = mediator;
            this.notificationService = notificationService;
            Handler = new(notificationService);
        }
    }
}
