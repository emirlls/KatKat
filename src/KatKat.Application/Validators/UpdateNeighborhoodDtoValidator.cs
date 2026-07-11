using FluentValidation;
using KatKat.Constants;
using KatKat.Dtos;

namespace KatKat.Validators;

public class UpdateNeighborhoodDtoValidator : AbstractValidator<UpdateNeighborhoodDto>
{
    public UpdateNeighborhoodDtoValidator()
    {
        RuleFor(x => x.DistrictId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(NeighborhoodConsts.MaxNameLength);
    }
}
