using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Payments;

public sealed class PaymentOwnershipException()
    : DomainException("You are not allowed to access this payment.")
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
    public override string Title => "Forbidden";
}