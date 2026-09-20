using Backend.Src.Application.Dtos.Enums.Profiles;

namespace Backend.Src.Application.Dtos.Requests.Auth;

public record ExternalSignUpRequest(
    string Code,
    ProfileType ProfileType
);