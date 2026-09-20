using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class PhoneNumberRequiredException(Guid profileId)
    : DomainException($"The profile {profileId} must have a Phone Number to continue this operation")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Phone Number required";
}