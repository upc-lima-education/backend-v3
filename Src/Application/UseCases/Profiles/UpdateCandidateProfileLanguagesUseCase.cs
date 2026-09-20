using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Application.Mappers.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Profiles;

public class UpdateCandidateProfileLanguagesUseCase(IProfileRepository profileRepository)
{
    public async Task ExecuteAsync(UpdateCandidateProfileLanguagesRequest request, Guid userId)
    {
        var profile = await profileRepository.GetByUserIdForUpdateAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        if (profile.CandidateProfile is null)
            throw new CandidateProfileRequiredException();
        
        var candidateProfile = profile.CandidateProfile;
        
        var languages = request.Languages.Select(x => LanguageKnownMapper.ToEntity(x, profile.Id)).ToList();
        profile.CandidateProfile.UpdateLanguages(languages);
        await profileRepository.UpdateAsync(profile);
    }
}