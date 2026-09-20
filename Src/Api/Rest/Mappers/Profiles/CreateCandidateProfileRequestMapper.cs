using Backend.Src.Api.Rest.Dtos.Profiles;
using Backend.Src.Application.Dtos.Requests.Common;
using Backend.Src.Application.Dtos.Requests.Profiles;

namespace Backend.Src.Api.Rest.Mappers.Profiles;

public static class CreateCandidateProfileRequestMapper
{
    public static CreateCandidateProfileRequest ToApplicationRequest(CreateCandidateProfileApiRequest request)
    {
        UploadProfilePictureRequest? profilePicture = null;
        if (request.ProfilePicture is not null)
        {
            var stream = request.ProfilePicture.OpenReadStream();
            profilePicture = new UploadProfilePictureRequest(
                new UploadFileRequest(
                    stream,
                    request.ProfilePicture.FileName,
                    request.ProfilePicture.ContentType
                )
            );
        }

        return new CreateCandidateProfileRequest(
            request.Description,
            request.Ubigeo,
            request.PhoneNumber,
            request.Skills,
            request.FirstName,
            request.LastName,
            request.Dni,
            profilePicture
        );
    }
}