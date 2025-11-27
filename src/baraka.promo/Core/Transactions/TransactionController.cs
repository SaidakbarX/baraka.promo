using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using baraka.promo.Models.LoyaltyApiModels.Transactions;
using baraka.promo.Core.Authorize;
using baraka.promo.Extensions;
using baraka.promo.Utils;

namespace baraka.promo.Core.Transactions
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController: ControllerBase
    {
        readonly IMediator _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<LoyaltyResultModel>), 200)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TransactionModel model)
        {
            if (!TryValidateModel(model)) return BadRequest();

            var command = new AddTransaction.Command(model);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<LoyaltyResultModel>), 200)]
        [Route("replenish")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public async Task<IActionResult> PostReplenish([FromBody] ReplenishmentModel model)
        {
            if (!TryValidateModel(model)) return BadRequest();

            var command = new ReplenishBalance.Command(model);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<LoyaltyResultModel>), 200)]
        [HttpDelete]
        public async Task<IActionResult> Cancel([FromQuery] string external_id)
        {
            var command = new CancelTransaction.Command(external_id);
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}
