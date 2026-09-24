using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Domain.Rules.Profiles;
using FluentValidation;

namespace Backend.Src.Application.Validators.Profiles;

internal sealed class UpdateCandidateProfileValidator : AbstractValidator<UpdateCandidateProfileRequest>
{
    private const int MinNameLength = CandidateProfileRules.MinNameLenth;
    private const int MaxNameLength = CandidateProfileRules.MaxNameLength;
    public UpdateCandidateProfileValidator()
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
            .When(x => !string.IsNullOrWhiteSpace(x.Dni))
            .WithMessage("DNI must contain exactly 8 digits");
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+[1-9]\d{6,14}$")
                .When(x => x.PhoneNumber is not null)
                .WithMessage("Must be a valid phone number");
    }
}