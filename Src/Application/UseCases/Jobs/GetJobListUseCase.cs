using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Application.Dtos.Responses.Jobs;
using Backend.Src.Application.Mappers.Jobs;

namespace Backend.Src.Application.UseCases.Jobs;

public class GetJobListUseCase(IJobRepository jobRepository)
{
    public async Task<IReadOnlyList<JobListItemResponse>> ExecuteAsync()
    {
        var jobs = await jobRepository.GetAllAsync();
        var response = jobs.Select(JobListItemResponseMapper.ToResponse).ToList();
        return response;
    }
}
