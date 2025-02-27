using Card.Domain.Entities;

namespace Card.Application.Interfaces
{
    /// <summary>
    /// Represents the card service interface that provides methods
    /// for verifying user and card existence and retrieving card details.
    /// </summary>
    public interface ICardService
    {
        /// <summary>
        /// Checks whether a user exists based on the provided user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to check for existence.</param>
        /// <returns>
        /// A boolean value indicating whether the user exists (true) or not (false).
        /// </returns>
        Task<bool> CheckUserExistenceByUserId(string userId);

        /// <summary>
        /// Checks whether a card exists for the specified user ID and card number.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to whom the card may belong.</param>
        /// <param name="cardNumber">The card number to check for existence.</param>
        /// <returns>
        /// A boolean value indicating whether the card exists (true) or not (false).
        /// </returns>
        Task<bool> CheckCardExistenceByCardNumber(string userId, string cardNumber);
        
        /// <summary>
        /// Retrieves card details for a given user and card number asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user who owns the card.</param>
        /// <param name="cardNumber">The card number to retrieve details for.</param>
        /// <returns>
        /// A <see cref="CardDetails"/> instance containing the card information if found; otherwise, null.
        /// </returns>
        Task<CardDetails?> GetCardDetailsAsync(string userId, string cardNumber);
    }
}