using Backend.Src.Application.Dtos.Data.Profiles;
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Domain.Rules.Profiles;
using FluentValidation;

namespace Backend.Src.Application.Validators.Profiles;

internal sealed class UpdateCandidateProfileWorkExperiencesValidator : AbstractValidator<UpdateCandidateProfileWorkExperiencesRequest>
{
    public UpdateCandidateProfileWorkExperiencesValidator()
    {
        RuleFor(x => x.WorkExperiences)
            .NotNull()
            .WithMessage("Work experiences cannot be null");

        RuleForEach(x => x.WorkExperiences)
            .SetValidator(new WorkExperienceDataValidator());
    }
}

internal sealed class WorkExperienceDataValidator : AbstractValidator<WorkExperienceData>
{
    private const int MaxCompanyLength = CandidateProfileRules.MaxCompanyLength;
    private const int MaxPositionLength = CandidateProfileRules.MaxPositionLength;
    private const int MaxDescriptionLength = CandidateProfileRules.MaxDescriptionLength;

    public WorkExperienceDataValidator()
    {
        RuleFor(x => x.Company)
            .NotEmpty().WithMessage("Company cannot be empty")
            .MaximumLength(MaxCompanyLength).WithMessage($"Company name must be at most {MaxCompanyLength} characters long");
        RuleFor(x => x.Position)
            .NotEmpty().WithMessage("Position cannot be empty")
            .MaximumLength(MaxPositionLength).WithMessage($"Position must be at most {MaxPositionLength} characters long");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description cannot be empty")
            .MaximumLength(MaxDescriptionLength).WithMessage($"Description must be at most {MaxDescriptionLength} characters long");
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.EndDate.HasValue).WithMessage("End date must be greater or equal than Start date");
    }
}