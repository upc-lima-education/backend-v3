using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Payments;

public sealed class PaymentAlreadyCompletedException(string orderId)
    : DomainException($"Payment '{orderId}' was already completed")
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Payment already completed";
}