using Backend.Src.Application.Dtos.Data.Profiles;
using Backend.Src.Domain.Entities.Profiles;

namespace Backend.Src.Application.Mappers.Profiles;

public static class EducationMapper
{
    public static Education ToEntity(EducationData data, Guid profileId)
    {
        return new Education(
            profileId,
            data.Institution,
            data.Degree,
            data.FieldOfStudy,
            data.StartDate,
            data.EndDate
        );
    }

    public static EducationData ToData(Education entity)
    {
        return new EducationData(
            entity.Institution,
            entity.Degree,
            entity.FieldOfStudy,
            entity.StartDate,
            entity.EndDate
        );
    }
}