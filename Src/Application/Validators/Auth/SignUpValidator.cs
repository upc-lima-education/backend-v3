using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Domain.Rules.Auth;
using FluentValidation;

namespace Backend.Src.Application.Validators.Auth;

internal sealed class SignUpValidator : AbstractValidator<SignUpRequest>
{
    private const int MinPasswordLength = UserRules.MinPasswordLength;
    private const int MaxPasswordLength = UserRules.MaxPasswordLength;

    public SignUpValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .EmailAddress().WithMessage("Email format is invalid");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(MinPasswordLength).WithMessage($"Password must be at least {MinPasswordLength} characters long")
            .MaximumLength(MaxPasswordLength).WithMessage($"Password must be at most {MaxPasswordLength} characters long")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"\d").WithMessage("Password must contain at least one digit")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

    }
}
