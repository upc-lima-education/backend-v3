using Backend.Src.Domain.Exceptions.Jobs;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Jobs;

public class ClaimJobUseCase(IJobRepository jobRepository, IProfileRepository profileRepository)
{
    public async Task ExecuteAsync(Guid jobId, Guid userId)
    {
         var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CompanyProfile is null) throw new CompanyProfileRequiredException();
        if (!profile.CompanyProfile.IsVerified) throw new CompanyNotVerifiedException();

        var job = await jobRepository.GetByIdForUpdateAsync(jobId)
            ?? throw new JobNotFoundException(jobId);
        
        job.ClaimJob(profile.CompanyProfile);
        await jobRepository.UpdateAsync(job);
    }
}