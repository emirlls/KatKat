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
        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage(_ => localizer[ValidationErrorCodes.ResourceReservation.EndTimeMustBeAfterStartTime]);
    }
}