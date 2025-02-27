using AutoMapper;
using Card.Application.Handlers;
using Card.Application.Interfaces;
using Card.Application.Models;
using Card.Application.Queries;
using Card.Domain.Entities;
using Card.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Card.Application.Tests.Handlers
{
    public class GetCardDetailsQueryHandlerTests
    {
        private readonly Mock<ICardService> _cardServiceMock;
        private readonly Mock<ICardActionService> _cardActionServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<GetCardDetailsQuery>> _validatorMock;
        private readonly GetCardDetailsQueryHandler _handler;

        public GetCardDetailsQueryHandlerTests()
        {
            _cardServiceMock = new Mock<ICardService>();
            _cardActionServiceMock = new Mock<ICardActionService>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<GetCardDetailsQuery>>();

            _handler = new GetCardDetailsQueryHandler(
                _cardServiceMock.Object,
                _cardActionServiceMock.Object,
                _mapperMock.Object,
                _validatorMock.Object
            );
        }

        [Fact]
        public async Task Handle_ReturnsCardAllowedActionsDto_WhenCardExists()
        {
            // Arrange
            const string cardNumberValue = "Card1";
            var request = new GetCardDetailsQuery { UserId = "User1", CardNumber = cardNumberValue };
            var cardDetails = new CardDetails(CardNumber: cardNumberValue, CardType: CardType.Credit, CardStatus: CardStatus.Active, IsPinSet: true);
            var allowedActions = new[] { "ACTION1", "ACTION2" };

            _validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _cardServiceMock
                .Setup(s => s.GetCardDetailsAsync(request.UserId, request.CardNumber))
                .ReturnsAsync(cardDetails);

            _cardActionServiceMock
                .Setup(a => a.GetAllowedActions(cardDetails.CardType, cardDetails.CardStatus))
                .ReturnsAsync(allowedActions);

            _mapperMock
                .Setup(m => m.Map<CardAllowedActionsDto>(It.IsAny<string[]>()))
                .Returns((string[] actions) => new CardAllowedActionsDto { AllowedActions = actions });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(allowedActions, result.AllowedActions);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenCardDoesNotExist()
        {
            // Arrange
            var request = new GetCardDetailsQuery { UserId = "User1", CardNumber = "Card1" };

            _validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _cardServiceMock
                .Setup(s => s.GetCardDetailsAsync(request.UserId, request.CardNumber))
                .ReturnsAsync((CardDetails)null!);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_ThrowsValidationException_WhenValidationFails()
        {
            // Arrange
            var request = new GetCardDetailsQuery { UserId = "", CardNumber = "" };

            var validationErrors = new List<ValidationFailure>
            {
                new ("UserId", "UserId is required"),
                new ("CardNumber", "CardNumber is required")
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationErrors));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(request, CancellationToken.None));
            Assert.Contains(exception.Errors, e => e.PropertyName == "UserId");
            Assert.Contains(exception.Errors, e => e.PropertyName == "CardNumber");
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new GetCardDetailsQuery { UserId = "NonExistingUser", CardNumber = "Card1" };

            _validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _cardServiceMock
                .Setup(s => s.CheckUserExistenceByUserId(request.UserId))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenCardDoesNotBelongToUser()
        {
            // Arrange
            var request = new GetCardDetailsQuery { UserId = "User1", CardNumber = "Card999" };

            _validatorMock
                .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _cardServiceMock
                .Setup(s => s.CheckUserExistenceByUserId(request.UserId))
                .ReturnsAsync(true);

            _cardServiceMock
                .Setup(s => s.CheckCardExistenceByCardNumber(request.UserId, request.CardNumber))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
