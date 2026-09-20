namespace Backend.Src.Domain.Contracts.Auth;

public record PasswordResetData(
    Guid UserId,
    bool IsVerified
);