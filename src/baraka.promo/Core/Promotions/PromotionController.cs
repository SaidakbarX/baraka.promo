using MediatR;
using Microsoft.AspNetCore.Mvc;
using baraka.promo.Models.OrderApiModel;
using baraka.promo.Core.Authorize;
using baraka.promo.Extensions;
using baraka.promo.Models;
using baraka.promo.Models.PromoApi;

namespace baraka.promo.Core.Promotions
{
    [Route("api/promotion")]
    [ApiController]
    [ApiKeyAuth]

    public class PromotionController: ControllerBase
    {
        readonly IMediator _mediator;

        public PromotionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ProducesResponseType(typeof(PreorderResultModel), 200)]
        [Route("AuthorizedPreorder")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PreorderModel model)
        {
            var api_key_id = HttpContext.GetApiKey();

            var command = new ApplyPromotions.Command(model, api_key_id);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result.Data);
            else return BadRequest();
        }

        [ProducesResponseType(typeof(ApiBaseResultModel), 200)]
        [Route("customer/register")]
        [HttpPost]
        public async Task<IActionResult> PostCustomer([FromBody] CustomerApiModel model)
        {
            var api_key_name = HttpContext.GetApiKeyName();

            var command = new RegisterCustomer.Command(model, api_key_name);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);
            else return BadRequest();
        }
    }
}
