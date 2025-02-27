using Card.Application.Interfaces;
using Card.Application.Queries;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Card.Application.Validators
{
    /// <summary>
    /// Validator for the <c>GetCardDetailsQuery</c>.
    /// </summary>
    /// <remarks>
    /// This validator ensures that the query contains valid and required data for processing.
    /// It verifies the presence of the <c>UserId</c> and <c>CardNumber</c> fields and
    /// validates their correctness by checking against the application's card service.
    /// </remarks>
    /// <example>
    /// The <c>GetCardDetailsQueryValidator</c> checks:
    /// <list type="bullet">
    /// <item><description>That <c>UserId</c> is not empty and corresponds to an existing user.</description></item>
    /// <item><description>That <c>CardNumber</c> is not empty and matches an existing card associated with the user.</description></item>
    /// </list>
    /// </example>
    public class GetCardDetailsQueryValidator : AbstractValidator<GetCardDetailsQuery>
    {
        private readonly ICardService _cardService;
        private readonly IStringLocalizer<GetCardDetailsQuery> _localizer;

        public GetCardDetailsQueryValidator(ICardService cardService, IStringLocalizer<GetCardDetailsQuery> localizer)
        {
            _cardService = cardService;
            _localizer = localizer;
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage(_localizer["UserIdRequired"].Value)
                .MustAsync(UserExists).WithMessage(_localizer["UserDoesNotExist"].Value);
            
            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage(_localizer["CardNumberRequired"].Value)
                .MustAsync(CardExists).WithMessage(_localizer["CardDoesNotExist"].Value);
        }

        /// <summary>
        /// Determines whether a user exists for the specified user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean value indicating
        /// whether the user exists (true) or not (false).
        /// </returns>
        private async Task<bool> UserExists(string userId, CancellationToken cancellationToken)
        {
            return await _cardService.CheckUserExistenceByUserId(userId);
        }

        /// <summary>
        /// Determines whether a card exists for the specified user ID and card number.
        /// </summary>
        /// <param name="query">The query object containing details such as user ID and card number.</param>
        /// <param name="cardNumber">The card number to check for existence.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean value indicating
        /// whether the card exists (true) or not (false).
        /// </returns>
        private async Task<bool> CardExists(GetCardDetailsQuery query, string cardNumber, CancellationToken cancellationToken)
        {
            return await _cardService.CheckCardExistenceByCardNumber(query.UserId, cardNumber);
        }
    }
}