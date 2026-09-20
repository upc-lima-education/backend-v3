using Backend.Src.Application.Dtos.Requests.Jobs;
using Backend.Src.Application.Dtos.Responses.Jobs;
using Backend.Src.Domain.Repositories.Jobs;
using FluentValidation;
using Backend.Src.Application.Resolvers.Skills;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Application.Mappers.Jobs;
using Backend.Src.Domain.Exceptions.Profiles;

namespace Backend.Src.Application.UseCases.Jobs;

public class CreateInternalJobUseCase(
    IJobRepository jobRepository,
    IProfileRepository profileRepository,
    SkillResolver skillResolver,
    IValidator<CreateInternalJobRequest> validator
)
{
    public async Task<JobResponse> ExecuteAsync(CreateInternalJobRequest request, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CompanyProfile is null) throw new CompanyProfileRequiredException();
        
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var skills = await skillResolver.ResolveAsync(request.Skills);

        var job = JobMapper.ToEntity(
            request,
            profile.Id,
            skills,
            profile.CompanyProfile.CompanyName,
            profile.ProfilePicture
        );
        await jobRepository.CreateAsync(job);
        job.AssociateCompany(profile.CompanyProfile);
        var response = JobMapper.ToResponse(job);
        return response;
    }
}
