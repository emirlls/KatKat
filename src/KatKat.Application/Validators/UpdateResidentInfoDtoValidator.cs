using FluentValidation;
using KatKat.Dtos;

namespace KatKat.Validators;

public class UpdateResidentInfoDtoValidator : AbstractValidator<UpdateResidentInfoDto>
{
    public UpdateResidentInfoDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
    }
}
