using System.Text;
using System.Text.Json;

namespace Backend.Src.Application.Dtos.Data.Auth;

public sealed record AuthenticationStateData(string Nonce, long Timestamp, string Mode, string? ProfileType)
{
    public AuthenticationStateData()
        : this(
            Guid.NewGuid().ToString(),
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            "login",
            null
        )
    { }

    public static string Encode(AuthenticationStateData state)
    {
        var json = JsonSerializer.Serialize(state);

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    public static AuthenticationStateData? Decode(string encodedState)
    {
        var base64 = encodedState
                .Replace('-', '+')
                .Replace('_', '/');

        base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));

        return JsonSerializer.Deserialize<AuthenticationStateData>(json);
    }
}
