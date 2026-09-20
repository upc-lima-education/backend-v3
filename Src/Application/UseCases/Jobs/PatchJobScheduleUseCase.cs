using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Application.Dtos.Requests.Jobs;
using FluentValidation;
using Backend.Src.Domain.Exceptions.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;

namespace Backend.Src.Application.UseCases.Jobs;

public class PatchJobScheduleUseCase(
    IJobRepository jobRepository,
    IProfileRepository profileRepository,
    IValidator<PatchJobScheduleRequest> validator
)
{
    public async Task ExecuteAsync(PatchJobScheduleRequest request, Guid jobId, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CompanyProfile is null) throw new CompanyProfileRequiredException();
        
        var job = await jobRepository.GetByIdForUpdateAsync(jobId)
            ?? throw new JobNotFoundException(jobId);
        
        if(job.CompanyId != profile.Id)
            throw new JobAccessDeniedException(jobId);

        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        
        job.UpdateSchedule(request.OpensAt, request.ClosesAt);
        await jobRepository.UpdateAsync(job);
    }
}
