using Card.Application.Interfaces;
using Card.Domain.Enums;

namespace Card.Infrastructure.Services
{
    /// <summary>
    /// Provides functionality to determine allowed actions for a given card type and status
    /// based on predefined rules and mappings.
    /// </summary>
    public sealed class CardActionService : ICardActionService
    {
        private readonly Dictionary<string, Dictionary<(CardType, CardStatus), bool>> _actionRules;

        public CardActionService()
        {
            _actionRules = CreateActionRulesDictionary();
        }

        /// <summary>
        /// Creates a dictionary where each action is mapped to a nested dictionary containing tuples of card type and card status as keys,
        /// and a boolean value indicating their association as the value.
        /// </summary>
        /// <returns>A dictionary where the key is an action name (string) and the value is a nested dictionary.
        /// The nested dictionary uses a tuple of card type and card status as keys, and a boolean as the value to indicate their association for the specific action.</returns>
        private Dictionary<string, Dictionary<(CardType, CardStatus), bool>> CreateActionRulesDictionary()
        {
            return new Dictionary<string, Dictionary<(CardType, CardStatus), bool>>
            {
                {
                    "ACTION1", new Dictionary<(CardType, CardStatus), bool>
                    {
                        { (CardType.Prepaid, CardStatus.Active), true },
                        { (CardType.Debit, CardStatus.Active), true },
                        { (CardType.Credit, CardStatus.Active), true }
                    }
                },
                {
                    "ACTION2", new Dictionary<(CardType, CardStatus), bool>
                    {
                        { (CardType.Prepaid, CardStatus.Inactive), true },
                        { (CardType.Debit, CardStatus.Inactive), true },
                        { (CardType.Credit, CardStatus.Inactive), true }
                    }
                },
                {
                    "ACTION3", GenerateForAllCardTypes([
                        CardStatus.Ordered, CardStatus.Inactive, CardStatus.Active, CardStatus.Restricted,
                        CardStatus.Blocked, CardStatus.Expired, CardStatus.Closed
                    ])
                },
                {
                    "ACTION4",
                    GenerateForAllCardTypes([
                        CardStatus.Ordered, CardStatus.Inactive, CardStatus.Active, CardStatus.Restricted,
                        CardStatus.Blocked, CardStatus.Expired, CardStatus.Closed
                    ])
                },
                {
                    "ACTION5",
                    GenerateForCardType(CardType.Credit,
                    [
                        CardStatus.Ordered, CardStatus.Inactive, CardStatus.Active, CardStatus.Restricted,
                        CardStatus.Blocked, CardStatus.Expired, CardStatus.Closed
                    ])
                },
                {
                    "ACTION6",
                    GenerateForAllCardTypes([CardStatus.Ordered, CardStatus.Inactive, CardStatus.Active])
                },
                {
                    "ACTION7",
                    GenerateForAllCardTypes([CardStatus.Ordered, CardStatus.Inactive, CardStatus.Active])
                },
                {
                    "ACTION8",
                    GenerateForAllCardTypes([CardStatus.Ordered, CardStatus.Inactive, CardStatus.Active])
                },
                {
                    "ACTION9",
                    GenerateForAllCardTypes([
                        CardStatus.Ordered, CardStatus.Inactive, CardStatus.Active, CardStatus.Restricted,
                        CardStatus.Blocked, CardStatus.Expired, CardStatus.Closed
                    ])
                },
                {
                    "ACTION10",
                    GenerateForAllCardTypes([CardStatus.Ordered, CardStatus.Inactive, CardStatus.Active])
                },
                {
                    "ACTION11", 
                    GenerateForAllCardTypes([CardStatus.Inactive, CardStatus.Active])
                },
                {
                    "ACTION12",
                    GenerateForAllCardTypes([CardStatus.Ordered, CardStatus.Inactive, CardStatus.Active])
                },
                {
                    "ACTION13",
                    GenerateForAllCardTypes([CardStatus.Ordered, CardStatus.Inactive, CardStatus.Active])
                }
            };
        }

        /// <summary>
        /// Generates a dictionary mapping all card types and the specified array of card statuses to a boolean value indicating their association.
        /// </summary>
        /// <param name="statuses">An array of card statuses to be associated with all available card types.</param>
        /// <returns>A dictionary where each key is a tuple consisting of a card type and a card status, and the value is a boolean indicating the association.</returns>
        private Dictionary<(CardType, CardStatus), bool> GenerateForAllCardTypes(CardStatus[] statuses)
        {
            var dict = new Dictionary<(CardType, CardStatus), bool>();
            foreach (CardType type in Enum.GetValues(typeof(CardType)))
            {
                foreach (var status in statuses)
                {
                    dict[(type, status)] = true;
                }
            }

            return dict;
        }

        /// <summary>
        /// Generates a dictionary mapping the specified card type and an array of card statuses to a boolean value indicating the association.
        /// </summary>
        /// <param name="type">The type of the card for which the mapping is being created.</param>
        /// <param name="statuses">An array of card statuses to be associated with the specified card type.</param>
        /// <returns>A dictionary where each key is a tuple of card type and card status, and the value is a boolean.</returns>
        private Dictionary<(CardType, CardStatus), bool> GenerateForCardType(CardType type, CardStatus[] statuses)
        {
            var dict = new Dictionary<(CardType, CardStatus), bool>();
            foreach (var status in statuses)
            {
                dict[(type, status)] = true;
            }

            return dict;
        }
        
        /// <summary>
        /// Retrieves a list of allowed actions for a specific card type and status based on predefined rules.
        /// </summary>
        /// <param name="cardType">The type of the card for which actions are being determined.</param>
        /// <param name="cardStatus">The current status of the card for which actions are being determined.</param>
        /// <returns>
        /// A task containing an array of allowed action names as strings. Returns an empty array if no actions are allowed.
        /// </returns>
        public Task<string[]> GetAllowedActions(CardType cardType, CardStatus cardStatus)
        {
            var allowedActions = _actionRules
                .AsParallel() // To perform operation for big range of data
                .Where(action => action.Value.TryGetValue((cardType, cardStatus), out bool isAllowed) && isAllowed)
                .Select(action => action.Key)
                .Order()
                .ToArray();

            return Task.FromResult(allowedActions);
        }
    }
}