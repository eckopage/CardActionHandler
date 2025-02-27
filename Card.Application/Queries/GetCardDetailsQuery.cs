using Card.Application.Models;
using MediatR;

namespace Card.Application.Queries
{
    /// <summary>
    /// Query object for retrieving card details based on user ID and card number.
    /// </summary>
    /// <remarks>
    /// This query is processed to fetch detailed information regarding a specific card
    /// linked to the user. The response will be encapsulated in a <c>CardDetailsDto</c>.
    /// </remarks>
    public class GetCardDetailsQuery : IRequest<CardAllowedActionsDto?>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user requesting the card details.
        /// This value is required to associate the request with a specific user.
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Gets or sets the unique card number associated with the card details request.
        /// This value is required to identify the specific card being queried.
        /// </summary>
        public required string CardNumber { get; set; }
    }
}