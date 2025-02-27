using Microsoft.AspNetCore.Mvc;
using MediatR;
using Card.API.Models;
using Card.Application.Models;
using Microsoft.Extensions.Localization;
using Card.Application.Queries;
using Swashbuckle.AspNetCore.Annotations;

namespace Card.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<CardController> _localizer;

        public CardController(IMediator mediator, IStringLocalizer<CardController> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }
        
        /// <summary>
        /// Retrieves details for a given card based on the user ID and card number.
        /// </summary>
        /// <param name="request">The card request containing user ID and card number.</param>
        /// <returns>Returns card details or an error message.</returns>
        /// <response code="200">Returns the card details.</response>
        /// <response code="400">If validation fails.</response>
        /// <response code="404">If the card is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
        [Route("GetCardDetails")]
        [SwaggerOperation(Summary = "Get card details", Description = "Retrieves details for a given card based on the user ID and card number.")]
        [ProducesResponseType(typeof(CardAllowedActionsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCardDetails([FromQuery] CardRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var query = new GetCardDetailsQuery
            {
                UserId = request.UserId,
                CardNumber = request.CardNumber
            };

            var result = await _mediator.Send(query);

            return result != null
                ? Ok(result)
                : NotFound(_localizer["CardNotFound"].Value);
        }
    }
}
