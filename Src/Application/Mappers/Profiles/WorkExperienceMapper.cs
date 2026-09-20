using Backend.Src.Application.Dtos.Data.Profiles;
using Backend.Src.Domain.Entities.Profiles;

namespace Backend.Src.Application.Mappers.Profiles;

public static class WorkExperienceMapper
{
    public static WorkExperience ToEntity(WorkExperienceData data, Guid profileId)
    {
        return new WorkExperience(
            profileId,
            data.Company,
            data.Position,
            data.Description,
            data.StartDate,
            data.EndDate
        );
    }

    public static WorkExperienceData ToData(WorkExperience entity)
    {
        return new WorkExperienceData(
            entity.Company,
            entity.Position,
            entity.Description,
            entity.StartDate,
            entity.EndDate
        );
    }
}