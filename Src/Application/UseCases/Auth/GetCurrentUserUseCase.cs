using Backend.Src.Application.Dtos.Data.Auth;
using Backend.Src.Application.Dtos.Enums.Profiles;
using Backend.Src.Application.Mappers.Auth;
using Backend.Src.Domain.Exceptions.Auth;
using Backend.Src.Domain.Exceptions.Profiles;
using Backend.Src.Domain.Repositories.Auth;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Auth;

public class GetCurrentUserUseCase(
    IUserRepository users,
    IProfileRepository profileRepository
)
{
    public async Task<UserData> ExecuteAsync(Guid userId)
    {
        var user = await users.GetByIdAsync(userId)
            ?? throw new UserNotFoundException(userId);
        var profile = await profileRepository.GetByUserIdAsync(userId)
            ?? throw new ProfileNotFoundException(userId);
        ProfileType profileType = ProfileType.Candidate;
        Guid profileId = profile.Id;
        if (profile.CompanyProfile is not null)
            profileType = ProfileType.Company;
        var response = UserResponseMapper.ToData(user, profileType, profileId);
        return response;
    }
}