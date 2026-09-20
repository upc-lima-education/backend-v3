using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Auth;

public sealed class EmailAlreadyExistsException(string email)
    : DomainException($"The email {email} is already in use")
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Email already exists";
}