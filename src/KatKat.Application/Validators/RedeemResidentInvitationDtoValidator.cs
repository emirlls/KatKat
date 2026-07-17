using FluentValidation;
using KatKat.Dtos;

namespace KatKat.Validators;

public class RedeemResidentInvitationDtoValidator : AbstractValidator<RedeemResidentInvitationDto>
{
    public RedeemResidentInvitationDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
