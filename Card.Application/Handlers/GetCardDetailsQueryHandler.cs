using AutoMapper;
using Card.Application.Interfaces;
using Card.Application.Models;
using Card.Application.Queries;
using FluentValidation;
using MediatR;

namespace Card.Application.Handlers
{
    /// <summary>
    /// Handles the execution of the <see cref="GetCardDetailsQuery"/> to retrieve card details.
    /// </summary>
    /// <remarks>
    /// This query handler processes the request by leveraging services such as <see cref="ICardService"/> and <see cref="ICardActionService"/>.
    /// The handler ensures that the input query is validated using a designated validator, performs any necessary business logic,
    /// and maps the resulting data into a <see cref="CardAllowedActionsDto"/> object.
    /// </remarks>
    public class GetCardDetailsQueryHandler(
        ICardService cardService,
        ICardActionService cardActionService,
        IMapper mapper,
        IValidator<GetCardDetailsQuery> validator)
        : IRequestHandler<GetCardDetailsQuery, CardAllowedActionsDto?>
    {
        /// <summary>
        /// Handles the incoming <see cref="GetCardDetailsQuery"/> to retrieve card details and associated actions.
        /// </summary>
        /// <param name="request">The <see cref="GetCardDetailsQuery"/> instance that contains the user ID and card number.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
        /// <returns>
        /// A <see cref="CardAllowedActionsDto"/> object containing the details of allowed actions for the card,
        /// or null if the card is not found or validation fails.
        /// </returns>
        public async Task<CardAllowedActionsDto?> Handle(GetCardDetailsQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var card = await cardService.GetCardDetailsAsync(request.UserId, request.CardNumber);
            if (card == null)
            {
                return null;
            }

            var allowedActions = await cardActionService.GetAllowedActions(card.CardType, card.CardStatus);
            return mapper.Map<CardAllowedActionsDto>(allowedActions);
        }
    }
}