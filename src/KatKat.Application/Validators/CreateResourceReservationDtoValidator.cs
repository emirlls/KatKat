using FluentValidation;
using KatKat.Constants;
using KatKat.Dtos;
using KatKat.Localization;
using Microsoft.Extensions.Localization;

namespace KatKat.Validators;

public class CreateResourceReservationDtoValidator : AbstractValidator<CreateResourceReservationDto>
{
    public CreateResourceReservationDtoValidator(IStringLocalizer<KatKatResource> localizer)
    {
        RuleFor(x => x.ResourceId).NotEmpty();
        // Allow start == end (a single-instant/day booking); only reject an end strictly before start.
        RuleFor(x => x.EndTime)
            .GreaterThanOrEqualTo(x => x.StartTime)
            .WithMessage(_ => localizer[ValidationErrorCodes.ResourceReservation.EndTimeMustBeAfterStartTime]);
    }
}