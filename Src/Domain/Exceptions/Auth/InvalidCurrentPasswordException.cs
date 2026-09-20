using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Auth;

public sealed class InvalidCurrentPasswordException()
    : DomainException("The password entered is incorrect")
{
    public override int StatusCode => StatusCodes.Status401Unauthorized;
    public override string Title => "Invalid password";
}