using Backend.Src.Application.Dtos.Requests.Curriculums;
using Backend.Src.Domain.Rules.Curriculums;
using FluentValidation;

namespace Backend.Src.Application.Validators.Curriculums;

internal sealed class CreateCvStructuredContentValidator : AbstractValidator<CreateCvStructuredContentRequest>
{
    public CreateCvStructuredContentValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(CvRules.MinTitleLength)
            .MaximumLength(CvRules.MaxTitleLength);
    }
}