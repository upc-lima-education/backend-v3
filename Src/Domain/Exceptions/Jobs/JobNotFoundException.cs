using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Jobs;

public sealed class JobNotFoundException(Guid jobId)
    : DomainException($"Job '{jobId}' was not found.")
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "Job not found";
}