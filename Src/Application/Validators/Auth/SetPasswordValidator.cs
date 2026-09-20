using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Domain.Rules.Auth;
using FluentValidation;

namespace Backend.Src.Application.Validators.Auth;

internal sealed class SetPasswordValidator : AbstractValidator<SetPasswordRequest>
{
    public SetPasswordValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(UserRules.MinPasswordLength)
            .MaximumLength(UserRules.MaxPasswordLength)
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"\d").WithMessage("Password must contain at least one digit")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}