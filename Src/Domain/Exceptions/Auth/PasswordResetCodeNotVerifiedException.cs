using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Auth;

public sealed class PasswordResetCodeNotVerifiedException()
    : DomainException("The password reset code provided was not verified")
{
    public override int StatusCode => StatusCodes.Status401Unauthorized;
    public override string Title => "Reset code unverified";
}