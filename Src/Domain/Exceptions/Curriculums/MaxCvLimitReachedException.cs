using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Curriculums;

public sealed class MaxCvLimitReachedException(int amount)
    : DomainException($"A candidate can only have {amount} CVs.")
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Maximum number of CVs reached";
}