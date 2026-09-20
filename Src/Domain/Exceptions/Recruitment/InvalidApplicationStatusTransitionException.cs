using Backend.Src.Domain.Exceptions.Common;
using Backend.Src.Domain.ValueObjects.Recruitment;

namespace Backend.Src.Domain.Exceptions.Recruitment;

public sealed class InvalidApplicationStatusTransitionException(ApplicationStatus currentStatus, ApplicationStatus newStatus)
    : DomainException($"Cannot transition from {currentStatus} to {newStatus}")
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Title => "Invalid Application Status transition";
}