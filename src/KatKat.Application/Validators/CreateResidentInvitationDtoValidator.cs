using FluentValidation;
using KatKat.Dtos;

namespace KatKat.Validators;

public class CreateResidentInvitationDtoValidator : AbstractValidator<CreateResidentInvitationDto>
{
    public CreateResidentInvitationDtoValidator()
    {
        RuleFor(x => x.FlatId).NotEmpty();
    }
}
