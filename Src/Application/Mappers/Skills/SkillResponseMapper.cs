using Backend.Src.Application.Dtos.Responses.Skills;
using Backend.Src.Domain.Entities.Skills;

namespace Backend.Src.Application.Mappers.Skills;

public static class SkillResponseMapper
{
    public static SkillResponse ToResponse(Skill skill)
    {
        return new SkillResponse(
            skill.Id,
            skill.Name
        );
    }
}