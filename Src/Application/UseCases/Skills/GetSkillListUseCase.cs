using Backend.Src.Application.Dtos.Responses.Skills;
using Backend.Src.Application.Mappers.Skills;
using Backend.Src.Domain.Repositories.Skills;

namespace Backend.Src.Application.UseCases.Skills;

public class GetSkillListUseCase(ISkillRepository skillRepository)
{
    public async Task<IReadOnlyList<SkillResponse>> ExecuteAsync()
    {
        var skills = await skillRepository.GetAll();
        var response = skills.Select(SkillResponseMapper.ToResponse).ToList();
        return response;
    }
}
