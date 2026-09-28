using Backend.Src.Application.Dtos.Enums.Profiles;
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Profiles;

public class CreateProfileUseCase(IProfileRepository profileRepository)
{
    public async Task<Profile> ExecuteAsync(CreateProfileRequest request)
    {
        var existingProfile = await profileRepository.GetByUserIdAsync(request.UserId);
        if (existingProfile is not null)
            throw new ProfileAlreadyExistsException(request.UserId.ToString(), "userId");

        var profile = new Profile(request.UserId);
        if (request.ProfileType is ProfileType.Company) profile.CompanyProfile = new CompanyProfile(profile.Id, profile);
        else profile.CandidateProfile = new CandidateProfile(profile.Id, profile);

        await profileRepository.CreateAsync(profile);
        return profile;
    }
}
