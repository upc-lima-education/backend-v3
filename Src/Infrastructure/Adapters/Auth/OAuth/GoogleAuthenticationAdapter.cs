using Backend.Src.Domain.Contracts.Auth;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Infrastructure.Contracts.Auth.OAuth;
using Backend.Src.Infrastructure.Options.Auth;
using Microsoft.Extensions.Options;

namespace Backend.Src.Infrastructure.Adapters.Auth.OAuth;

public class GoogleAuthenticationAdapter(
    HttpClient httpClient,
    IOptions<GoogleAuthOptions> options
) : IExternalAuthenticationPort
{
    /// <summary>
    /// Set authorization parameters. <br/>
    /// See: https://developers.google.com/identity/protocols/oauth2/web-server#creatingclient
    /// </summary>
    // 
    public string GetAuthorizationUrl(string? state = null)
    {
        const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        var parameters = new Dictionary<string, string?>
        {
            ["client_id"] = options.Value.ClientId,
            ["redirect_uri"] = options.Value.RedirectUri,
            ["response_type"] = "code",
            ["scope"] = "openid email profile",
            ["access_type"] = "offline",
            ["prompt"] = "select_account",
            ["state"] = state
        };

        var query = string.Join("&",
            parameters
                .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                .Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value!)}")
        );

        var url = $"{AuthorizationEndpoint}?{query}";
        return url;
    }
    
    /// <summary>
    /// Exchange code for access token and ID token <br/>
    /// See: https://developers.google.com/identity/openid-connect/openid-connect#exchangecode
    /// </summary>
    public async Task<string?> GetExchangeCodeAsync(string code)
    {
        const string TokenEndpoint = "https://oauth2.googleapis.com/token";
        var body = new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = options.Value.ClientId,
            ["client_secret"] = options.Value.ClientSecret,
            ["redirect_uri"] = options.Value.RedirectUri,
            ["grant_type"] = "authorization_code",
        };

        var response = await httpClient.PostAsync(TokenEndpoint, new FormUrlEncodedContent(body));
        if (!response.IsSuccessStatusCode) return null;

        var data = await response.Content.ReadFromJsonAsync<GoogleAccessTokenResponse>();
        if (data is null || string.IsNullOrWhiteSpace(data.TokenId)) return null;

        var tokenId = data.TokenId;
        return tokenId;
    }

    /// <summary>
    /// Getting the user identity from their google account <br/>
    /// See: https://developers.google.com/identity/openid-connect/openid-connect#obtainuserinfo <br/>
    /// See also: https://developers.google.com/identity/openid-connect/openid-connect#validatinganidtoken
    /// </summary>
    public async Task<ExternalUserIdentity?> GetUserIdentityAsync(string idToken)
    {
        var url = $"https://oauth2.googleapis.com/tokeninfo?id_token={Uri.EscapeDataString(idToken)}";
        
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;

        var data = await response.Content.ReadFromJsonAsync<GoogleIdTokenResponse>();
        if (data is null) return null;

        //Audience MUST equal to ClientId
        if (!string.Equals(data.Audience, options.Value.ClientId, StringComparison.Ordinal)) return null;

        //Issuer MUST be google
        if (data.Issuer is not ("accounts.google.com" or "https://accounts.google.com")) return null;

        //Token MUST NOT have expired
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (data.ExpiresAtUnix <= now) return null;

        var userIdentity = new ExternalUserIdentity(
            data.Email,
            data.IsEmailVerified,
            data.FirstName,
            data.LastName,
            data.Picture
        );

        return userIdentity;
    }
}
