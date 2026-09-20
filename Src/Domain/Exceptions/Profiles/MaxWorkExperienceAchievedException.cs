using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class MaxWorkExperienceAchievedException(int MaxLength)
    : DomainException($"A profile must have at most {MaxLength} work related experiences")
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Title => "Max Work Experience Achieved";
}