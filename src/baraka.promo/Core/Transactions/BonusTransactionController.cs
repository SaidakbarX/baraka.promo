using baraka.promo.Core.Authorize;
using baraka.promo.Extensions;
using baraka.promo.Models;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models.LoyaltyApiModels.Transactions;
using baraka.promo.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace baraka.promo.Core.Transactions
{
    [Route("api/transaction/bonus")]
    [ApiController]
    [ApiKeyAuth]

    public class BonusTransactionController : ControllerBase
    {
        readonly IMediator _mediator;

        public BonusTransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<LoyaltyBonusResultModel>), 200)]
        [HttpPost]
        public async Task<IActionResult> PostBonus([FromBody] TransactionBonusModel model)
        {
            var api_key_name = HttpContext.GetApiKeyName();

            var command = new AddBonusTransaction.Command(model, api_key_name);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);

            else
            {
                if (result.Error.Code == ErrorHeplerType.ERROR_UNAUTHORIZED.ToString()) return Unauthorized();
                else return BadRequest(result.Error);
            }
        }

        [ProducesResponseType(200)]
        [HttpDelete]
        public async Task<IActionResult> Cancel([FromQuery] string external_id)
        {
            var command = new CancelBonusTransaction.Command(external_id);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok();

            else
            {
                if (result.Error.Code == ErrorHeplerType.ERROR_UNAUTHORIZED.ToString()) return Unauthorized();
                else return BadRequest(result.Error);
            }
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<ListBaseModel<TransactionBonusInfoModel>>), 200)]
        [HttpGet]
        public async Task<IActionResult> GetUser([FromBody] TransactionRequestModel model)
        {
            var command = new GetBonusTransactions.Command(model);
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
