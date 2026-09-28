using Backend.Src.Application.Dtos.Enums.Profiles;

namespace Backend.Src.Application.Dtos.Requests.Profiles;

public record CreateProfileRequest(
    Guid UserId,
    ProfileType ProfileType
);