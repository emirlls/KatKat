using FluentValidation;
using KatKat.Dtos;

namespace KatKat.Validators;

public class UpdateManagerDtoValidator : AbstractValidator<UpdateManagerDto>
{
    public UpdateManagerDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
    }
}
