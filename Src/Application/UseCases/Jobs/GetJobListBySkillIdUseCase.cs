using Backend.Src.Application.Dtos.Responses.Jobs;
using Backend.Src.Application.Mappers.Jobs;
using Backend.Src.Domain.Repositories.Skills;

namespace Backend.Src.Application.UseCases.Jobs;

public class GetJobListBySkillIdUseCase(ISkillRepository skillRepository)
{
    public async Task<IReadOnlyList<JobListItemResponse>> ExecuteAsync(Guid skillId)
    {
        var skill = await skillRepository.GetByIdWithJobs(skillId);
        if (skill is null) return [];
        var jobs = skill.Jobs;
        if (jobs is null) return [];
        var response = jobs.Select(JobListItemResponseMapper.ToResponse).ToList();
        return response;
    }
}