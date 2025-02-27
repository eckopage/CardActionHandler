namespace Card.Application.Models
{
    /// <summary>
    /// Represents the details of a card including its number, type, status, and PIN settings.
    /// </summary>
    public class CardDetailsDto
    {
        /// <summary>
        /// Represents the type of the card, such as credit, debit, or prepaid.
        /// This is a required property used to categorize the card.
        /// </summary>
        public required string CardType { get; set; }

        /// <summary>
        /// Indicates the current status of the card, such as whether it is
        /// active, inactive, or blocked. This is a required property and
        /// is utilized to determine the operational state of the card.
        /// </summary>
        public required string CardStatus { get; set; }

        /// <summary>
        /// Indicates whether a PIN is set for the card.
        /// This property determines if the card has an active PIN configured.
        /// </summary>
        public bool IsPinSet { get; set; }
    }
}