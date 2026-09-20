using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Recruitment;

public sealed class InternalJobRequiredException(Guid jobId)
    : DomainException($"Job '{jobId}' does not accept applications within Llanqui.")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Internal Llanqui job required";
}
