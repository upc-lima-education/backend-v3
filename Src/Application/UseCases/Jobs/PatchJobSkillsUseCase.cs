using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Application.Dtos.Requests.Jobs;
using Backend.Src.Application.Resolvers.Skills;
using FluentValidation;
using Backend.Src.Domain.Exceptions.Jobs;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;

namespace Backend.Src.Application.UseCases.Jobs;

public class PatchJobSkillsUseCase(
    IJobRepository jobRepository,
    IProfileRepository profileRepository,
    SkillResolver skillResolver,
    IValidator<PatchJobSkillsRequest> validator
)
{
    public async Task ExecuteAsync(PatchJobSkillsRequest request, Guid jobId, Guid userId)
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

        var skills = await skillResolver.ResolveAsync(request.Skills);
        job.UpdateJobSkills(skills);
        await jobRepository.UpdateAsync(job);
    }
}