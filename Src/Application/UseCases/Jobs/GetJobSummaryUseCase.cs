using Backend.Src.Application.Dtos.Responses.Jobs;
using Backend.Src.Domain.Repositories.Jobs;

namespace Backend.Src.Application.UseCases.Jobs;

public sealed class GetJobSummaryUseCase(IJobRepository jobRepository)
{
    public async Task<JobSummaryResponse> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var totalActiveJobs = await jobRepository.GetActiveCountAsync(cancellationToken);
        return new JobSummaryResponse(totalActiveJobs);
    }
}
