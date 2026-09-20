using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Auth;

public sealed class PasswordAlreadySetException()
    : DomainException("A password has already been set for this account")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Password already set";
}