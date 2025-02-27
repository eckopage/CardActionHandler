using Card.Domain.Enums;

namespace Card.Application.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that determines the allowed actions for cards
    /// based on their type and current status.
    /// </summary>
    public interface ICardActionService
    {
        /// <summary>
        /// Determines and retrieves a list of allowed actions based on the type and status of the card.
        /// </summary>
        /// <param name="cardType">
        /// Specifies the type of the card, such as Prepaid, Debit, or Credit.
        /// </param>
        /// <param name="cardStatus">
        /// Specifies the current status of the card, such as Active, Blocked, or Expired.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a list of strings representing the allowed actions.
        /// </returns>
        Task<string[]> GetAllowedActions(CardType cardType, CardStatus cardStatus);
    }
}