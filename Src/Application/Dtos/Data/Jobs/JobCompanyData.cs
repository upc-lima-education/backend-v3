namespace Backend.Src.Application.Dtos.Data.Jobs;

public record JobCompanyData(
    Guid Id,
    string Name,
    string? ImageUrl,
    bool IsVerified
);