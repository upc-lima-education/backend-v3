using Backend.Src.Domain.ValueObjects.Profiles;

namespace Backend.Src.Application.Dtos.Data.Profiles;

public record LanguageKnownData(
    LanguageCode Code,
    LanguageLevel Level
);