using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class CompanyNotVerifiedException()
    : DomainException("The company is not verified")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Company not verified";
}