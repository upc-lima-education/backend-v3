namespace Backend.Src.Application.Dtos.Data.Auth;

public record UserData(
    Guid Id,
    string Email,
    bool EmailVerified,
    string? ProfileType,
    Guid? ProfileId,
    bool IsActive,
    DateTime CreatedAt
);
