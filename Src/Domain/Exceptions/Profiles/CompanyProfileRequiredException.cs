using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class CompanyProfileRequiredException()
    : DomainException("A company profile is required to perform this action.")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Company profile required";
}