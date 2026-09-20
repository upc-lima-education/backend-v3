using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class CandidateProfileRequiredException()
    : DomainException("A candidate profile is required to perform this action.")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Candidate profile required";
}