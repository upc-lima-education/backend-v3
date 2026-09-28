using Backend.Src.Application.Dtos.Enums.Profiles;

namespace Backend.Src.Application.Dtos.Data.Auth;

public record UserData(
    Guid Id,
    string Email,
    bool EmailVerified,
    ProfileType ProfileType,
    Guid ProfileId,
    bool IsActive,
    DateTime CreatedAt
);
