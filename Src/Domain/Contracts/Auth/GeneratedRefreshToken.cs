namespace Backend.Src.Domain.Contracts.Auth;

public record GeneratedRefreshToken(
    string Token,
    string Jti,
    TimeSpan Expiration
);