using Backend.Src.Domain.Exceptions.Common;

namespace Backend.Src.Domain.Exceptions.Profiles;

public sealed class LanguageKnownNotFoundException()
    : DomainException($"A language code/level combination not found")
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string Title => "Language not found";
}