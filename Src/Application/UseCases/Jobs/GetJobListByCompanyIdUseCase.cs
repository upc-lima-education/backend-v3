using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Application.Dtos.Responses.Jobs;
using Backend.Src.Application.Mappers.Jobs;

namespace Backend.Src.Application.UseCases.Jobs;

public class GetJobListByCompanyIdUseCase(IJobRepository jobRepository)
{
    public async Task<IReadOnlyList<JobListItemResponse>> ExecuteAsync(Guid companyId)
    {
        var jobs = await jobRepository.GetAllByCompanyIdAsync(companyId);
        var response = jobs.Select(JobListItemResponseMapper.ToResponse).ToList();
        return response;
    }
}