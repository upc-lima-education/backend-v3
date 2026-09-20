using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Domain.Rules.Profiles;
using FluentValidation;

namespace Backend.Src.Application.Validators.Profiles;

internal sealed class CreateCandidateProfileValidator : AbstractValidator<CreateCandidateProfileRequest>
{
    private const int MinNameLength = CandidateProfileRules.MinNameLenth;
    private const int MaxNameLength = CandidateProfileRules.MaxNameLength;
    public CreateCandidateProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty")
            .MinimumLength(MinNameLength).WithMessage($"First name must be at least {MinNameLength} characters long")
            .MaximumLength(MaxNameLength).WithMessage($"First name must be at most {MaxNameLength} characters long");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty")
            .MinimumLength(MinNameLength).WithMessage($"Last name must be at least {MinNameLength} characters long")
            .MaximumLength(MaxNameLength).WithMessage($"Last name must be at most {MaxNameLength} characters long");
        RuleFor(x => x.Dni)
            .Matches(@"^\d{8}$")
            .When(x => x.Dni is not null)
            .WithMessage("DNI must contain exactly 8 digits");
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+[1-9]\d{6,14}$")
            .WithMessage("Must be a valid phone number");
    }
}