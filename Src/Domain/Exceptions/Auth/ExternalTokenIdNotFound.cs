using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Auth;

public sealed class ExternalTokenIdNotFoundException()
    : DomainException("Could not get Token Id from external provider")
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Title => "Invalid external token";
}