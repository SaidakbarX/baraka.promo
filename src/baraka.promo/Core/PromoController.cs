using baraka.promo.Core.Authorize;
using baraka.promo.Models;
using baraka.promo.Models.LoyaltyApiModels.FilterModels;
using baraka.promo.Models.OrderApiModel;
using baraka.promo.Models.Paging;
using baraka.promo.Models.PromoApi;
using baraka.promo.Models.Segment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace baraka.promo.Core
{
    [Route("api/promo/[action]")]
    [ApiController]
    //[ApiKeyAuth]

    public class PromoController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ToExport _export;
        public PromoController(IMediator mediator, ToExport export)
        {
            _mediator = mediator;
            _export = export;
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<OrderApiResultModel>), 200)]
        [HttpPost]
        public async Task<IActionResult> Get([FromBody] OrderModel model)
        {
            var command = new GetPromo.Command(model);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<PromoApiResultModel>), 200)]
        [HttpPost]
        public async Task<IActionResult> GetByName([FromBody] OrderPromoModel model)
        {
            var command = new GetPromoByName.Command(model);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [ProducesResponseType(typeof(ApiBaseResultModel), 200)]
        [HttpPost]
        public async Task<IActionResult> Applied([FromBody] AppliedPromoModel model)
        {
            var command = new UserAppliedPromo.Command(model);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<List<PromoApiResultModel>>), 200)]
        [HttpGet]
        public async Task<IActionResult> GetByPhone([FromQuery] string phone)
        {
            var command = new GetUserPromos.Command(phone);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [ProducesResponseType(typeof(ApiBaseResultModel), 200)]
        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] AddClientPromoModel model)
        {
            var command = new AddClientPromo.Command(model);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [ProducesResponseType(200)]
        [HttpGet]
        public async Task<IActionResult> GetSegments()
        {
            var command = new GetSegments.Command();
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<PageResultModel<string>>), 200)]
        [HttpGet]
        public async Task<IActionResult> GetPhoneBySegmentId([FromQuery] SegmentUserFilterModel filter)
        {
            var command = new GetSegmentUsers.Command(filter);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public async Task<IActionResult> Export([FromBody] SegmentUserFilterModel filter)
        {
            filter.Skip = filter.Take = 0;
            var command = new GetExportData.Command(filter);
            var result = await _mediator.Send(command);
            if (result != null && result?.Data?.Value != null)
            {
                var items = result?.Data?.Value.Select(s => new
                {
                    s.PhoneNumber,
                    s.FirstOrderTime,
                    s.LastOrderTime,
                    s.FromLastOrderToNow,
                    s.OrderAmountCount,
                    s.TotalOrdersAmount,
                    s.ContactName
                });

                

                return _export.ToExcel(items.AsQueryable());
            }
            else
                return NotFound();
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<ListBaseModel<PromoModel>>), 200)]
        [HttpPost]      
        public async Task<IActionResult> GetPromos([FromBody] PageFilterModel model)
        {
            var command = new GetPromos.Command(model);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [ProducesResponseType(typeof(ApiBaseResultModel<PromoResultModel>), 200)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PromoApiModel model)
        {
            var command = new AddPromo.Command(model);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }

        [ProducesResponseType(typeof(ApiBaseResultModel), 200)]
        [HttpPatch]
        public async Task<IActionResult> SetActive(long id)
        {
            var command = new SetActivePromo.Command(id);
            var result = await _mediator.Send(command);

            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }
    }
}