using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Auth;

public sealed class InvalidCredentialsException()
    : DomainException("Invalid user-password combination")
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Title => "Invalid Credentials";
}