using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Application.Mappers.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Profiles;
using FluentValidation;

namespace Backend.Src.Application.UseCases.Profiles;

public class UpdateCandidateProfileEducationsUseCase(
    IProfileRepository profileRepository,
    IValidator<UpdateCandidateProfileEducationsRequest> validator
)
{
    public async Task ExecuteAsync(UpdateCandidateProfileEducationsRequest request, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdForUpdateAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();

        var result = await validator.ValidateAsync(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var candidateProfile = profile.CandidateProfile;
        var educations = request.Educations.Select(x => EducationMapper.ToEntity(x, candidateProfile.ProfileId)).ToList();
        candidateProfile.UpdateEducations(educations);
        await profileRepository.UpdateAsync(profile);
    }
}