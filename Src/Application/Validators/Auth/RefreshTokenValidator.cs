using Backend.Src.Application.Dtos.Requests.Auth;
using FluentValidation;

namespace Backend.Src.Application.Validators.Auth;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token cannot be empty");
    }
}