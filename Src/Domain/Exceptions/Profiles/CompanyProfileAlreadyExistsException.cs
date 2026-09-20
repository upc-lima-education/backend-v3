using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class CompanyProfileAlreadyExistsException(Guid profileId)
    : DomainException($"Profile {profileId} already has a company profile associated to it")
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Company profile already exists";
}