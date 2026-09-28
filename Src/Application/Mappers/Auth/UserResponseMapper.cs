using Backend.Src.Application.Dtos.Data.Auth;
using Backend.Src.Application.Dtos.Enums.Profiles;
using Backend.Src.Domain.Entities.Auth;

namespace Backend.Src.Application.Mappers.Auth;

public static class UserResponseMapper
{
    public static UserData ToData(User user, ProfileType profileType, Guid profileId)
    {
        return new UserData(
            user.Id,
            user.Email,
            user.IsEmailVerified,
            profileType,
            profileId,
            user.IsActive,
            user.CreatedAt
        );
    }
}
