using System;
using FluentValidation;
using KatKat.Constants;
using KatKat.Dtos;
using KatKat.Localization;
using Microsoft.Extensions.Localization;

namespace KatKat.Validators;

public class ExtendComplexSubscriptionDtoValidator : AbstractValidator<ExtendComplexSubscriptionDto>
{
    public ExtendComplexSubscriptionDtoValidator(IStringLocalizer<KatKatResource> localizer)
    {
        RuleFor(x => x.NewEndDate)
            .GreaterThan(_ => DateTime.UtcNow)
            .WithMessage(_ => localizer[ValidationErrorCodes.ComplexSubscription.ExtendSubscriptionDateMustBeFuture]);
    }
}
