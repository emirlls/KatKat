using FluentValidation;
using KatKat.Constants;
using KatKat.Dtos;
using KatKat.Localization;
using Microsoft.Extensions.Localization;

namespace KatKat.Validators;

public class UpdateBuildingDtoValidator : AbstractValidator<UpdateBuildingDto>
{
    public UpdateBuildingDtoValidator(IStringLocalizer<KatKatResource> localizer)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(BuildingConsts.MaxNameLength);
        RuleFor(x => x.FloorCount)
            .GreaterThan(0)
            .When(x => x.FloorCount.HasValue)
            .WithMessage(_ => localizer[ValidationErrorCodes.Building.FloorCountMustBePositive]);
    }
}
