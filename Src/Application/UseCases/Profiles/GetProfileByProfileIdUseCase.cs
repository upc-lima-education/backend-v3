using Backend.Src.Application.Mappers.Profiles;
using Backend.Src.Domain.Repositories.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Application.Dtos.Responses.Profiles;

namespace Backend.Src.Application.UseCases.Profiles;

public class GetProfileByProfileIdUseCase(IProfileRepository profileRepository)
{
    public async Task<ProfileResponse> ExecuteAsync(Guid profileId)
    {
        var profile = await profileRepository.GetByIdAsync(profileId)
            ?? throw new ProfileNotFoundException(profileId);
        return ProfileMapper.ToResponse(profile);
    }
}
