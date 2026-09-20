using Backend.Src.Domain.ValueObjects.Profiles;

namespace Backend.Src.Domain.Entities.Profiles;

public sealed class LanguageKnown
{
    public Guid CandidateProfileId { get; private set; }
    public LanguageCode LanguageCode { get; private set; }
    public LanguageLevel LanguageLevel { get; private set; }

    public LanguageKnown() { }

    public LanguageKnown(
        Guid profileId,
        LanguageCode languageCode,
        LanguageLevel languageLevel
    )
    {
        CandidateProfileId = profileId;
        LanguageCode = languageCode;
        LanguageLevel = languageLevel;
    }
}