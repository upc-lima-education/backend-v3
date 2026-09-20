using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Payments;

public sealed class InsufficientCreditsException(int currentAmount, int amountNeeded)
    : DomainException($"The provided amount {currentAmount} is lower than {amountNeeded}")
{
    public override int StatusCode => StatusCodes.Status402PaymentRequired;
    public override string Title => "Insufficient credits";
}