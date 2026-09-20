using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Jobs;

public sealed class JobAccessDeniedException(Guid jobId)
    : DomainException($"You do not have access to job '{jobId}'.")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Job access denied";
}