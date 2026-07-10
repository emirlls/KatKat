using FluentValidation;
using KatKat.Constants;
using KatKat.Dtos;
using KatKat.Localization;
using Microsoft.Extensions.Localization;

namespace KatKat.Validators;

public class CreateExpenseDtoValidator : AbstractValidator<CreateExpenseDto>
{
    public CreateExpenseDtoValidator(IStringLocalizer<KatKatResource> localizer)
    {
        RuleFor(x => x.ComplexId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(ExpenseConsts.MaxTitleLength);
        RuleFor(x => x.Description).MaximumLength(ExpenseConsts.MaxDescriptionLength);
        RuleFor(x => x.ReceiptImageUrl).MaximumLength(ExpenseConsts.MaxReceiptImageUrlLength);
        RuleFor(x => x.TotalAmount)
            .GreaterThan(0)
            .WithMessage(_ => localizer[ValidationErrorCodes.Expense.TotalAmountMustBePositive]);
    }
}
