using FluentValidation;
using KatKat.Constants;
using KatKat.Dtos;

namespace KatKat.Validators;

public class CreateDistrictDtoValidator : AbstractValidator<CreateDistrictDto>
{
    public CreateDistrictDtoValidator()
    {
        RuleFor(x => x.CityId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(DistrictConsts.MaxNameLength);
    }
}
