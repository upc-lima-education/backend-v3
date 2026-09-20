using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class MaxLanguagesAchievedException(int MaxLength)
    : DomainException($"A profile must have at most {MaxLength} languages")
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string Title => "Max Languages Achieved";
}