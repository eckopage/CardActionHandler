using FluentValidation;
using Card.API.Models;
using Microsoft.Extensions.Localization;

namespace Card.API.Validators
{
    /// <summary>
    /// Validates properties of the <see cref="CardRequest"/> model using FluentValidation.
    /// </summary>
    /// <remarks>
    /// This class enforces validation rules for <see cref="CardRequest"/>.
    /// It ensures that required properties are not null or empty, and applies localized error messages
    /// for any validation failures.
    /// </remarks>
    public class CardRequestValidator : AbstractValidator<CardRequest>
    {
        public CardRequestValidator(IStringLocalizer<CardRequestValidator> localizer)
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage(localizer["UserIdRequired"]);
            RuleFor(x => x.CardNumber).NotEmpty().WithMessage(localizer["CardNumberRequired"]);
        }
    }
}