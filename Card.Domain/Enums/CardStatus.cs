namespace Card.Domain.Enums
{
    /// <summary>
    /// An Enumeration of Card Statuses.
    /// </summary>
    public enum CardStatus
    {
        /// <summary>
        /// Status when a card order has been placed but card is not yet active.
        /// </summary>
        Ordered,

        /// <summary>
        /// Status when card is not active. Reasons could be either it's newly ordered or has been deactivated temporarily.
        /// </summary>
        Inactive,

        /// <summary>
        /// Status when card is active and can be used for transactions.
        /// </summary>
        Active,

        /// <summary>
        /// Status when card is active but with certain usage restrictions.
        /// </summary>
        Restricted,

        /// <summary>
        /// Status when card is blocked due to suspicious activity and cannot be used.
        /// </summary>
        Blocked,

        /// <summary>
        /// Status when card has reached its expiry date and is no longer usable.
        /// </summary>
        Expired,
        
        /// <summary>
        /// Status when card has been closed or cancelled by the user or system.
        /// </summary>
        Closed
    }
}
