using System.Text.Json.Serialization;

namespace Backend.Src.Infrastructure.Contracts.Auth.OAuth;

internal sealed record GoogleAccessTokenResponse(
    [property: JsonPropertyName("access_token")]
    string AccessToken,

    [property: JsonPropertyName("expires_in")]
    int ExpiresIn,

    [property: JsonPropertyName("id_token")]
    string? TokenId,

    [property: JsonPropertyName("scope")]
    string Scope,

    [property: JsonPropertyName("token_type")]
    string TokenType,

    [property: JsonPropertyName("refresh_token")]
    string? RefreshToken
);