namespace Backend.Src.Application.Dtos.Data.Profiles;

public record CompanyProfileData(
    string? CompanyName,
    string? Sector,
    string? Ruc,
    string? Website,
    string? CompanySize,
    bool? IsVerified
);
