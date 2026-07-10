using FluentValidation;
using KatKat.Constants;
using KatKat.Dtos;

namespace KatKat.Validators;

public class CreateComplexDtoValidator : AbstractValidator<CreateComplexDto>
{
    public CreateComplexDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(ComplexConsts.MaxNameLength);
        RuleFor(x => x.City).NotEmpty().MaximumLength(ComplexConsts.MaxCityLength);
        RuleFor(x => x.District).NotEmpty().MaximumLength(ComplexConsts.MaxDistrictLength);
        RuleFor(x => x.Address).MaximumLength(ComplexConsts.MaxAddressLength);
        RuleFor(x => x.Latitude).InclusiveBetween(GeoConsts.MinLatitude, GeoConsts.MaxLatitude);
        RuleFor(x => x.Longitude).InclusiveBetween(GeoConsts.MinLongitude, GeoConsts.MaxLongitude);
    }
}
