namespace Card.Domain.Enums
{
    /// <summary>
    /// Represents the type of card in the system.
    /// </summary>
    /// <remarks>
    /// The types include:
    /// - Prepaid: A card that requires funds to be loaded before use.
    /// - Debit: A card directly linked to a bank account for transactions.
    /// - Credit: A card that allows purchases using borrowed funds within a credit limit.
    /// </remarks>
    public enum CardType
    {
        Prepaid,
        Debit,
        Credit
    }
}