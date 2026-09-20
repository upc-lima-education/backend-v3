using Backend.Src.Application.Dtos.Data.Auth;

namespace Backend.Src.Application.Dtos.Responses.Auth;

public record AuthenticationResponse(
    UserData User,
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    SuggestedProfileData? SuggestedProfileData = null
);