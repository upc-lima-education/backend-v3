using Backend.Src.Application.Dtos.Data.Auth;
using Backend.Src.Domain.Entities.Auth;

namespace Backend.Src.Application.Mappers.Auth;

public static class UserResponseMapper
{
    public static UserData ToResponse(User user, string? profileType = null, Guid? profileId = null)
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
