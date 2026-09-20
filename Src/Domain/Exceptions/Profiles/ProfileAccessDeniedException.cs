using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class ProfileAccessDeniedException(Guid userId)
    : DomainException($"You do not have access to profile with user id '{userId}'.")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Profile access denied";
}