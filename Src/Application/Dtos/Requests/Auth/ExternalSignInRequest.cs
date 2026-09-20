using Backend.Src.Application.Dtos.Enums.Profiles;

namespace Backend.Src.Application.Dtos.Requests.Auth;

public record ExternalSignInRequest(
    string Code,
    ProfileType ProfileType
);