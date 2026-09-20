using Backend.Src.Application.Dtos.Enums.Profiles;

namespace Backend.Src.Application.Dtos.Requests.Profiles;

public record ExternalCreateProfileRequest(
    Guid UserId,
    ProfileType? ProfileType,
    string FirstName,
    string LastName,
    string ProfilePicture
);