using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Recruitment;

public sealed class JobApplicationAccessDeniedException()
    : DomainException($"You do not have access to job application")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Job access denied";
}