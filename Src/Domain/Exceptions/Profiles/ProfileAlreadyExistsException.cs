using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class ProfileAlreadyExistsException(string identifier, string type)
    : DomainException($"Profile with {type} {identifier} already exists")
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Profile already exists";
}