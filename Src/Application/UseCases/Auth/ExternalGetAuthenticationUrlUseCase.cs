using Backend.Src.Application.Dtos.Data.Auth;
using Backend.Src.Application.Dtos.Enums.Profiles;
using Backend.Src.Domain.Ports.Auth;

namespace Backend.Src.Application.UseCases.Auth;

public class ExternalGetAuthenticationUrlUseCase(IExternalAuthenticationPort googleAuthService)
{
    public string Execute(string? mode, string? userType)
    {
        var normalizedMode = string.Equals(mode, "signup", StringComparison.OrdinalIgnoreCase)
            ? "signup"
            : "login";
        ProfileType? profileType = userType?.ToLowerInvariant() switch
        {
            "employee" => ProfileType.Candidate,
            "organization" => ProfileType.Company,
            _ => null
        };
        var oauthState = new AuthenticationStateData(
            Guid.NewGuid().ToString(),
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            normalizedMode,
            profileType?.ToString()
        );
        var encodedState = AuthenticationStateData.Encode(oauthState);
        var url = googleAuthService.GetAuthorizationUrl(encodedState);
        return url;
    }
}
