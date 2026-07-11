using FluentValidation;
using KatKat.Constants;
using KatKat.Dtos;

namespace KatKat.Validators;

public class UpdateCityDtoValidator : AbstractValidator<UpdateCityDto>
{
    public UpdateCityDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(CityConsts.MaxNameLength);
    }
}
