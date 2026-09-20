using System.Text.Json.Serialization;

namespace Backend.Src.Infrastructure.Contracts.Payments.Paypal;

internal record PayPalTokenResponse(
    [property: JsonPropertyName("access_token")]
    string AccessToken,

    [property: JsonPropertyName("token_type")]
    string TokenType,

    [property: JsonPropertyName("expires_in")]
    int ExpiresIn
);