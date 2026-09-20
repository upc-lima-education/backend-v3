namespace Backend.Src.Infrastructure.Options.Auth;

public class JwtOptions
{
    public string SecretKey { get; init; } = null!;
    public string RefreshSecretKey { get; init; } = null!;
    public string Issuer { get; init; } = "llanqui-api";
    public string Audience { get; init; } = "llanqui-api";
    public double AccessExpiresHours { get; init; } = 1;
    public double RefreshExpiresDays { get; init; } = 7;
}