using FluentValidation;
using KatKat.Dtos;
using KatKat.Resources;

namespace KatKat.Validators;

public class CreateResourceDtoValidator : AbstractValidator<CreateResourceDto>
{
    public CreateResourceDtoValidator()
    {
        RuleFor(x => x.ComplexId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(ResourceConsts.MaxNameLength);
    }
}
