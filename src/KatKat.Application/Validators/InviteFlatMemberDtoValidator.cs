using FluentValidation;
using KatKat.Dtos;

namespace KatKat.Validators;

public class InviteFlatMemberDtoValidator : AbstractValidator<InviteFlatMemberDto>
{
    public InviteFlatMemberDtoValidator()
    {
        RuleFor(x => x.FlatId).NotEmpty();
    }
}
