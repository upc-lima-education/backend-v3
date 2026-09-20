using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Payments;

public sealed class PlanNotFoundException(string planName)
    : DomainException($"The plan '{planName}' was not found.")
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "Plan not found";
}