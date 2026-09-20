using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Payments;

public sealed class PaymentNotFoundException(string orderId)
    : DomainException($"Payment '{orderId}' was not found.")
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "Payment not found";
}