using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Auth;

public sealed class ExternalUserIdentityNotFoundException()
    : DomainException("Could not get User Identity from external provider")
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Title => "Invalid external user identity";
}