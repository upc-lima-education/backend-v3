using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Recruitment;

public sealed class JobNotActiveException(Guid jobId)
    : DomainException($"Job '{jobId}' is not active. Only the owner can see it")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Job not active";
}