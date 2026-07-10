using FluentValidation;
using KatKat.Dtos;

namespace KatKat.Validators;

public class ReportSosAlertDtoValidator : AbstractValidator<ReportSosAlertDto>
{
    public ReportSosAlertDtoValidator()
    {
        RuleFor(x => x.ComplexId).NotEmpty();
        RuleFor(x => x.FlatId).NotEmpty();
    }
}
