using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Auth;

public sealed class InvalidRefreshTokenException()
    : DomainException("The token provided is invalid")
{
    public override int StatusCode => StatusCodes.Status401Unauthorized;
    public override string Title => "Invalid Refresh Token";
}