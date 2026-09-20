using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Recruitment;

public sealed class AlreadyAppliedToJobException(Guid jobId)
    : DomainException($"Already applied to Job {jobId}")
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Already applied to job";
}