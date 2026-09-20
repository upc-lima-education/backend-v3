using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class CandidateProfileAlreadyExistsException(Guid profileId)
    : DomainException($"Profile {profileId} already has a candidate profile associated to it")
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Candidate profile already exists";
}