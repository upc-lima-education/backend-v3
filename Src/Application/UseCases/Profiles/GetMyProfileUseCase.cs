using Backend.Src.Application.Dtos.Responses.Profiles;
using Backend.Src.Application.Mappers.Profiles;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Profiles;

public class GetMyProfileUseCase(IProfileRepository profileRepository)
{
    public async Task<ProfileResponse> ExecuteAsync(Guid userId)
    {
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        return ProfileMapper.ToResponse(profile);
    }
}
