using System;
using FluentValidation;
using KatKat.Constants;
using KatKat.Dtos;
using KatKat.Localization;
using KatKat.P2PRequests;
using Microsoft.Extensions.Localization;

namespace KatKat.Validators;

public class CreateP2PRequestDtoValidator : AbstractValidator<CreateP2PRequestDto>
{
    public CreateP2PRequestDtoValidator(IStringLocalizer<KatKatResource> localizer)
    {
        RuleFor(x => x.ComplexId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(P2PRequestConsts.MaxTitleLength);
        RuleFor(x => x.Description).MaximumLength(P2PRequestConsts.MaxDescriptionLength);
        RuleFor(x => x.NeededUntil)
            .GreaterThan(_ => DateTime.UtcNow)
            .When(x => x.NeededUntil.HasValue)
            .WithMessage(_ => localizer[ValidationErrorCodes.P2PRequest.NeededUntilMustBeFuture]);
    }
}
