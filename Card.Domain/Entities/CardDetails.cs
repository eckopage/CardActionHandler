using Card.Domain.Enums;

namespace Card.Domain.Entities
{
    /// <summary>
    /// Represents the details of a card within the system.
    /// </summary>
    /// <remarks>
    /// The CardDetails record encapsulates critical card-related information, including:
    /// - The card number (typically a unique identifier for the card).
    /// - The type of the card, which can be Prepaid, Debit, or Credit, as defined by <see cref="Card.Domain.Enums.CardType"/>.
    /// - The current status of the card, which may include values such as Active, Blocked, Expired, etc., as defined by <see cref="Card.Domain.Enums.CardStatus"/>.
    /// - Whether a personal identification number (PIN) has been set for the card.
    /// </remarks>
    public record CardDetails(string CardNumber, CardType CardType, CardStatus CardStatus, bool IsPinSet);
}