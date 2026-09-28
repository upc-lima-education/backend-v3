using Backend.Src.Application.Dtos.Enums.Profiles;

namespace Backend.Src.Application.Dtos.Requests.Auth;

public record SignUpRequest(
    string Email,
    string Password,
    ProfileType ProfileType
);