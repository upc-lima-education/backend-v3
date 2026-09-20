using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class SelfCompanyVerificationForbiddenException()
    : DomainException("Companies cannot verify their own profile.")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Self verification forbidden";
}
