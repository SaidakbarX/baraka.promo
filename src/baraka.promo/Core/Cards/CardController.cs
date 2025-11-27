using baraka.promo.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using baraka.promo.Models.LoyaltyApiModels.Cards;
using baraka.promo.Utils;
using baraka.promo.Services;

namespace baraka.promo.Core.Cards
{
    [Route("api/card")]
    [ApiController]

    public class CardController: ControllerBase
    {
        readonly IMediator _mediator;
        readonly ICurrentUser _current_user;

        public CardController(IMediator mediator, ICurrentUser currentUser)
        {
            _mediator = mediator;
            _current_user = currentUser;
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<CardInfoModel>), 200)]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? card_id, [FromQuery] string? card_number)
        {
            var user = _current_user.GetCurrentUserName();
            if (user == null) return Unauthorized();
            if (!_current_user.IsAdmin()) return BadRequest(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));

            var command = new GetCard.Command(card_id, card_number);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
