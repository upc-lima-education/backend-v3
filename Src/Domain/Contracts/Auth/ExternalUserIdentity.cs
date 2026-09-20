namespace Backend.Src.Domain.Contracts.Auth;

public sealed record ExternalUserIdentity(
    string Email,
    bool IsEmailVerified,
    string? FirstName,
    string? LastName,
    string? PictureUrl
);