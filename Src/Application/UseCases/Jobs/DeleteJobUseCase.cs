using Backend.Src.Domain.Exceptions.Jobs;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Jobs;

public class DeleteJobUseCase(
    IJobRepository jobRepository,
    IProfileRepository profileRepository
)
{
    public async Task ExecuteAsync(Guid jobId, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CompanyProfile is null) throw new CompanyProfileRequiredException();

        var job = await jobRepository.GetByIdAsync(jobId)
            ?? throw new JobNotFoundException(jobId);
        
        if(job.CompanyId != profile.Id) throw new JobAccessDeniedException(jobId);
        
        await jobRepository.DeleteAsync(jobId);
    }
}
