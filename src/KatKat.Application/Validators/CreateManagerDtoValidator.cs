using FluentValidation;
using KatKat.Dtos;

namespace KatKat.Validators;

public class CreateManagerDtoValidator : AbstractValidator<CreateManagerDto>
{
    public CreateManagerDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Site).NotNull().SetValidator(new CreateComplexDtoValidator());
    }
}
