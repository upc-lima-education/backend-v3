using Backend.Src.Application.Dtos.Enums.Profiles;

namespace Backend.Src.Application.Dtos.Requests.Auth;

public record ExternalAuthenticationRequest(
    string Code,
    string Mode,
    ProfileType? ProfileType
);
