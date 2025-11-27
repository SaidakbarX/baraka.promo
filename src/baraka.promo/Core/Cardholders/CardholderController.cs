using baraka.promo.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using baraka.promo.Models.LoyaltyApiModels.Cardholders;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models.Paging;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Core.Authorize;
using baraka.promo.Utils;

namespace baraka.promo.Core.Cardholders
{
    [Route("api/cardholder")]
    [ApiController]

    public class CardholderController : ControllerBase
    {
        readonly IMediator _mediator;

        public CardholderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[ProducesResponseType(typeof(ApiBaseResultModel<LoyaltyResultModel>), 200)]
        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] CardholderModel model)
        //{
        //    if (!TryValidateModel(model)) return BadRequest();

        //    var command = new AddCardholder.Command(model);
        //    var result = await _mediator.Send(command);

        //    return Ok(result);
        //}

        [ProducesResponseType(typeof(ApiBaseResultModel<CardholderInfoModel>), 200)]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? cardholder_id, [FromQuery] string? cardholder_phone, [FromQuery] string? card_id, [FromQuery] string? card_number)
        {
            var command = new GetCardholder.Command(cardholder_id, cardholder_phone, card_id, card_number);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<ApiBaseResultModel<ListBaseModel<CardholderInfoModel>>>), 200)]
        [Route("list")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FilterModel filter)
        {
            var command = new GetCardholders.Command(filter);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
