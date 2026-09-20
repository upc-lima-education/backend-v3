using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Auth;

public sealed class UserNotFoundException(Guid userId)
    : DomainException($"User {userId} not found")
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "User not found";
}