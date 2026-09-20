using System.Text.Json.Serialization;

namespace Backend.Src.Infrastructure.Contracts.Auth.OAuth;

public sealed record GoogleIdTokenResponse
(
    [property: JsonPropertyName("aud")]
    string Audience,

    [property: JsonPropertyName("exp")]
    [property: JsonConverter(typeof(FlexibleLongJsonConverter))]
    long ExpiresAtUnix,

    [property: JsonPropertyName("iss")]
    string Issuer,

    [property: JsonPropertyName("sub")]
    string Subject,

    [property:JsonPropertyName("email")]
    string Email,

    [property: JsonPropertyName("email_verified")]
    [property: JsonConverter(typeof(FlexibleBoolJsonConverter))]
    bool IsEmailVerified,

    [property:JsonPropertyName("name")]
    string? Name,

    [property: JsonPropertyName("given_name")]
    string? FirstName,

    [property: JsonPropertyName("family_name")]
    string? LastName,

    [property: JsonPropertyName("picture")]
    string? Picture,

    [property: JsonPropertyName("locale")]
    string? Locale
);