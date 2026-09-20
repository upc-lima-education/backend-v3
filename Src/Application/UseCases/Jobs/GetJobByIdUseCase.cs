using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Application.Mappers.Jobs;
using Backend.Src.Domain.Exceptions.Jobs;
using Backend.Src.Application.Dtos.Responses.Jobs;

namespace Backend.Src.Application.UseCases.Jobs;

public class GetJobByIdUseCase(IJobRepository jobRepository)
{
    public async Task<JobResponse> ExecuteAsync(Guid jobId)
    {
        var job = await jobRepository.GetByIdAsync(jobId)
            ?? throw new JobNotFoundException(jobId);
        var response = JobMapper.ToResponse(job);
        return response;
    }
}