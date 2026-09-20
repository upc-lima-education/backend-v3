using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class DuplicateLanguageException()
    : DomainException($"A profile must only contain distinct languages")
{
    public override int StatusCode => StatusCodes.Status409Conflict;
    public override string Title => "Duplicate language";
}