using baraka.promo.Core.Authorize;
using baraka.promo.Core.Messages;
using baraka.promo.Models.PushModels;
using baraka.promo.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using baraka.promo.Models.SubscriptionsModels;
using baraka.promo.Extensions;
using baraka.promo.Models.ClickModels;

namespace baraka.promo.Core.Subscriptions
{
    [Route("api/subscription")]
    [ApiController]

    public class SubscriptionController: ControllerBase
    {
        readonly IMediator _mediator;

        public SubscriptionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<SubscriptionApiModel>), 200)]
        [HttpGet]
        [Route("list")]
        [ApiKeyAuth]

        public async Task<IActionResult> Get([FromQuery] int country_id)
        {
            var command = new GetSubscriptions.Command(country_id);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [ProducesResponseType(typeof(ApiBaseResultModel), 200)]
        [HttpPost]
        [Route("receipt")]
        [ApiKeyAuth]

        public async Task<IActionResult> PostReceipt([FromBody] SubscriptionCheckModel model)
        {
            var api_key_name = HttpContext.GetApiKeyName();

            var command = new AddSubscriptionCheck.Command(model, api_key_name);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [HttpPost]
        [Route("{merchantId}/check")]
        public async Task<IActionResult> PostCheck(int merchantId, [FromBody] ClickPrepareModel model)
        {
            var command = new CheckSubscription.Command(model, merchantId);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
