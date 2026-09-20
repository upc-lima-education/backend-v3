using Backend.Src.Application.Dtos.Data.Profiles;
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Domain.Rules.Profiles;
using FluentValidation;

namespace Backend.Src.Application.Validators.Profiles;

internal sealed class UpdateCandidateProfileEducationsValidator : AbstractValidator<UpdateCandidateProfileEducationsRequest>
{
    public UpdateCandidateProfileEducationsValidator()
    {
        RuleFor(x => x.Educations)
            .NotNull()
            .WithMessage("Work experiences cannot be null");

        RuleForEach(x => x.Educations)
            .SetValidator(new EducationDataValidator());
    }
}

internal sealed class EducationDataValidator : AbstractValidator<EducationData>
{
    private const int MaxInstitutionLength = CandidateProfileRules.MaxInstitutionLength;
    private const int MaxDegreeLength = CandidateProfileRules.MaxDegreeLength;
    private const int MaxFieldOfStudyLength = CandidateProfileRules.MaxFieldOfStudyLength;

    public EducationDataValidator()
    {
        RuleFor(x => x.Institution)
            .NotEmpty().WithMessage("Institution cannot be empty")
            .MaximumLength(MaxInstitutionLength).WithMessage($"Institution name must be at most {MaxInstitutionLength} characters long");
        RuleFor(x => x.Degree)
            .NotEmpty().WithMessage("Degree cannot be empty")
            .MaximumLength(MaxDegreeLength).WithMessage($"Degree must be at most {MaxDegreeLength} characters long");
        RuleFor(x => x.FieldOfStudy)
            .MaximumLength(MaxFieldOfStudyLength)
                .When(x => x.FieldOfStudy is not null).WithMessage($"Description must be at most {MaxFieldOfStudyLength} characters long");
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.EndDate.HasValue).WithMessage("End date must be greater or equal than Start date");
    }
}