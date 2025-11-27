using baraka.promo.Core.Authorize;
using baraka.promo.Models.Cards;
using baraka.promo.Models.LoyaltyApiModels.Cards;
using baraka.promo.Models.LoyaltyApiModels.LoyalityTypeModels;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using baraka.promo.Extensions;

namespace baraka.promo.Core.Cards
{
    [Route("card")]
    [ApiController]
    [ApiKeyAuth]

    public class ApiCardController: ControllerBase
    {
        readonly IMediator _mediator;

        public ApiCardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<LoyaltyResultModel>), 200)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CardModel model)
        {
            var api_key_name = HttpContext.GetApiKeyName();

            var command = new AddCard.Command(model, api_key_name);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);

            else
            {
                if (result.Error.Code == ErrorHeplerType.ERROR_UNAUTHORIZED.ToString()) return Unauthorized();
                else return BadRequest(result.Error);
            }
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<LoyaltyCardResultModel>), 200)]
        [Route("authorize")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserCardModel model)
        {
            var api_key_name = HttpContext.GetApiKeyName();

            var command = new AddUserCard.Command(model, api_key_name);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);

            else
            {
                if (result.Error.Code == ErrorHeplerType.ERROR_UNAUTHORIZED.ToString()) return Unauthorized();
                else return BadRequest(result.Error);
            }
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<CardInfoModel>), 200)]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? card_id, [FromQuery] string? card_number)
        {
            var command = new GetCard.Command(card_id, card_number);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);

            else
            {
                if (result.Error.Code == ErrorHeplerType.ERROR_UNAUTHORIZED.ToString()) return Unauthorized();
                else return BadRequest(result.Error);
            }
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<CardInfoModel>), 200)]
        [Route("holder")]
        [HttpGet]
        public async Task<IActionResult> GetUser([FromQuery] string? cardholder_id)
        {
            var command = new GetUserCard.Command(cardholder_id);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);

            else
            {
                if (result.Error.Code == ErrorHeplerType.ERROR_UNAUTHORIZED.ToString()) return Unauthorized();
                else return BadRequest(result.Error);
            }
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<List<SubscriptionModel>>), 200)]
        [Route("loyalty")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var command = new GetSubscriptionCards.Command();
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);

            else
            {
                if (result.Error.Code == ErrorHeplerType.ERROR_UNAUTHORIZED.ToString()) return Unauthorized();
                else return BadRequest(result.Error);
            }
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<TokenModel>), 200)]
        [Route("{card_id}/token")]
        [HttpPost]
        public async Task<IActionResult> Post(Guid card_id)
        {
            var command = new GenerateCardToken.Command(card_id);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);

            else
            {
                if (result.Error.Code == ErrorHeplerType.ERROR_UNAUTHORIZED.ToString()) return Unauthorized();
                else return BadRequest(result.Error);
            }
        }
    }
}
