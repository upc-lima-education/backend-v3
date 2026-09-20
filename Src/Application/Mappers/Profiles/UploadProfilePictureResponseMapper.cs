using Backend.Src.Application.Dtos.Responses.Profiles;
using Backend.Src.Domain.Entities.Profiles;

namespace Backend.Src.Application.Mappers.Profiles;

public static class UploadProfilePictureResponseMapper
{
    public static UploadProfilePictureResponse ToResponse(Profile profile)
    {
        return new UploadProfilePictureResponse(
            profile.Id,
            profile.UserId,
            profile.ProfilePicture,
            profile.UpdatedAt
        );
    }
}