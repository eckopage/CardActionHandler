using Card.API.Controllers;
using Card.API.Models;
using Card.Application.Models;
using Card.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;

namespace Card.API.Tests.Controllers
{
    public class CardControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IStringLocalizer<CardController>> _localizerMock;
        private readonly CardController _controller;

        public CardControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _localizerMock = new Mock<IStringLocalizer<CardController>>();
            _controller = new CardController(_mediatorMock.Object, _localizerMock.Object);
        }

        [Fact]
        public async Task GetCardDetails_ReturnsOk_WhenCardExists()
        {
            // Arrange
            var request = new CardRequest { UserId = "user123", CardNumber = "1234" };
            var cardDetails = new CardAllowedActionsDto { AllowedActions = ["ACTION1", "ACTION2"] };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetCardDetailsQuery>(q => 
                    q.UserId == request.UserId && 
                    q.CardNumber == request.CardNumber), CancellationToken.None))
                .ReturnsAsync(cardDetails);

            // Act
            var result = await _controller.GetCardDetails(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCard = Assert.IsType<CardAllowedActionsDto>(okResult.Value);
            
            Assert.NotNull(returnedCard.AllowedActions);
            Assert.Contains("ACTION1", returnedCard.AllowedActions);
            Assert.Contains("ACTION2", returnedCard.AllowedActions);
        }

        [Fact]
        public async Task GetCardDetails_ReturnsNotFound_WhenCardDoesNotExist()
        {
            // Arrange
            var request = new CardRequest { UserId = "user123", CardNumber = "5678" };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetCardDetailsQuery>(), CancellationToken.None))
                .ReturnsAsync((CardAllowedActionsDto)null!);

            _localizerMock
                .Setup(l => l["CardNotFound"])
                .Returns(new LocalizedString("CardNotFound", "Card not found"));

            // Act
            var result = await _controller.GetCardDetails(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Card not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetCardDetails_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = new CardRequest();
            _controller.ModelState.AddModelError("UserId", "UserId is required");

            // Act
            var result = await _controller.GetCardDetails(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsType<SerializableError>(badRequestResult.Value);
            
            Assert.True(errors.ContainsKey("UserId"));
        }
    }
}
