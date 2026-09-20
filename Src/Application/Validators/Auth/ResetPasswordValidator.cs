using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Domain.Rules.Auth;
using FluentValidation;

namespace Backend.Src.Application.Validators.Auth;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
{
    private const int MinPasswordLength = UserRules.MinPasswordLength;
    private const int MaxPasswordLength = UserRules.MaxPasswordLength;

    public ResetPasswordValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(MinPasswordLength).WithMessage($"Password must be at least {MinPasswordLength} characters long")
            .MaximumLength(MaxPasswordLength).WithMessage($"Password must be at most {MaxPasswordLength} characters long")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"\d").WithMessage("Password must contain at least one digit")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}