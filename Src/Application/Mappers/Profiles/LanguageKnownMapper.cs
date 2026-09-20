using Backend.Src.Application.Dtos.Data.Profiles;
using Backend.Src.Domain.Entities.Profiles;

namespace Backend.Src.Application.Mappers.Profiles;

public static class LanguageKnownMapper
{
    public static LanguageKnown ToEntity(LanguageKnownData data, Guid candidateId)
    {
        return new LanguageKnown(
            candidateId,
            data.Code,
            data.Level
        );
    }

    public static LanguageKnownData ToData(LanguageKnown entity)
    {
        return new LanguageKnownData(
            entity.LanguageCode,
            entity.LanguageLevel
        );
    }
}