using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Application.Mappers.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Profiles;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Profiles;

public class UpdateCandidateProfileWorkExperiencesUseCase(
    IProfileRepository profileRepository,
    IValidator<UpdateCandidateProfileWorkExperiencesRequest> validator
)
{
    public async Task ExecuteAsync(UpdateCandidateProfileWorkExperiencesRequest request, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdForUpdateAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();
        
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var experiences = request.WorkExperiences.Select(x => WorkExperienceMapper.ToEntity(x, profile.Id)).ToList();
        profile.CandidateProfile.UpdateWorkExperiences(experiences);
        await profileRepository.UpdateAsync(profile);
    }
}