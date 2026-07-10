using FluentValidation;
using KatKat.Constants;
using KatKat.Dtos;

namespace KatKat.Validators;

public class CreateIssueDtoValidator : AbstractValidator<CreateIssueDto>
{
    public CreateIssueDtoValidator()
    {
        RuleFor(x => x.ComplexId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(IssueConsts.MaxTitleLength);
        RuleFor(x => x.Description).MaximumLength(IssueConsts.MaxDescriptionLength);
        RuleFor(x => x.PhotoUrl).MaximumLength(IssueConsts.MaxPhotoUrlLength);
    }
}
