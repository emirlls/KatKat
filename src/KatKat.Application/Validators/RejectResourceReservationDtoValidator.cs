using FluentValidation;
using KatKat.Constants;
using KatKat.Dtos;

namespace KatKat.Validators;

public class RejectResourceReservationDtoValidator : AbstractValidator<RejectResourceReservationDto>
{
    public RejectResourceReservationDtoValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(ReservationConsts.MaxRejectionReasonLength);
    }
}
