namespace Card.Application.Models;

public class CardAllowedActionsDto
{
    /// <summary>
    /// Gets or sets the collection of actions that are permitted for the card.
    /// </summary>
    public string[] AllowedActions { get; set; } = Array.Empty<string>();
}