namespace Backend.Src.Domain.Contracts.Auth;

public record RefreshTokenData(
    Guid UserId,
    string Jti
);