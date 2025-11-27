using baraka.promo.Core.Authorize;
using baraka.promo.Extensions;
using baraka.promo.Models.TgMessageModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace baraka.promo.Core.Messages
{
    [Route("api/message")]
    [ApiController]
    [ApiKeyAuth]

    public class MessageController: ControllerBase
    {
        readonly IMediator _mediator;

        public MessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ProducesResponseType(typeof(MessageModel), 200)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MessageModel model)
        {
            var api_key_name = HttpContext.GetApiKeyName();

            var command = new MessageSender.Command(model, api_key_name);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
