using FluentValidation;
using KatKat.Constants;
using KatKat.Dtos;
using KatKat.Localization;
using Microsoft.Extensions.Localization;

namespace KatKat.Validators;

public class UpdateFlatDtoValidator : AbstractValidator<UpdateFlatDto>
{
    public UpdateFlatDtoValidator(IStringLocalizer<KatKatResource> localizer)
    {
        RuleFor(x => x.FlatNumber).NotEmpty().MaximumLength(FlatConsts.MaxFlatNumberLength);
        RuleFor(x => x.ShareFactor)
            .GreaterThan(0)
            .WithMessage(_ => localizer[ValidationErrorCodes.Flat.ShareFactorMustBePositive]);
    }
}
