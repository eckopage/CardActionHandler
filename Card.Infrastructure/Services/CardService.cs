using Card.Application.Interfaces;
using Card.Domain.Entities;
using Card.Domain.Enums;

namespace Card.Infrastructure.Services
{
    public class CardService : ICardService
    {
        /// <summary>
        /// Initializes the CardService with a simulated in-memory database of user cards.
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, CardDetails>> _userCards;

        public CardService()
        {
            _userCards = CreateSampleUserCards();
        }

        public async Task<bool> CheckUserExistenceByUserId(string userId)
        {
            await Task.Delay(100);
            return _userCards.ContainsKey(userId);
        }

        public async Task<bool> CheckCardExistenceByCardNumber(string userId, string cardNumber)
        {
            await Task.Delay(100);
  
            if (!_userCards.TryGetValue(userId, out var cards))
            {
                return false;
            }

            return cards.ContainsKey(cardNumber);
        }
        
        public async Task<CardDetails?> GetCardDetailsAsync(string userId, string cardNumber)
        { 
            await Task.Delay(100); 
            return _userCards.TryGetValue(userId, out var cards) && cards.TryGetValue(cardNumber, out var cardDetails) ? cardDetails : null;
        }

        /// <summary>
        /// Creates a sample set of user cards to simulate a database.
        /// </summary>
        /// <returns>A dictionary of user cards.</returns>
        private static Dictionary<string, Dictionary<string, CardDetails>> CreateSampleUserCards()
        {
            var userCards = new Dictionary<string, Dictionary<string, CardDetails>>();
            for (var i = 1; i <= 3; i++)
            {
                var cards = new Dictionary<string, CardDetails>();
                var cardIndex = 1;
                foreach (CardType cardType in Enum.GetValues(typeof(CardType)))
                {
                    foreach (CardStatus cardStatus in Enum.GetValues(typeof(CardStatus)))
                    {
                        var cardNumber = $"Card{i}{cardIndex}";
                        cards.Add(cardNumber,
                            new CardDetails(
                                CardNumber: cardNumber,
                                CardType: cardType,
                                CardStatus: cardStatus,
                                IsPinSet: cardIndex % 2 == 0));
                        cardIndex++;
                    }
                }

                var userId = $"User{i}";
                userCards.Add(userId, cards);
            }

            return userCards;
        }
    }
}