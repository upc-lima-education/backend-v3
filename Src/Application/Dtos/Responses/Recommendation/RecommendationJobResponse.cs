namespace Backend.Src.Application.Dtos.Responses.Recommendation;

public sealed record RecommendationJobResponse(
    Guid JobId,
    string Title,
    string CompanyName,
    string? Ubigeo,
    decimal? MinSalary,
    decimal? MaxSalary,
    string? SourceUrl,
    double Score,
    string? JobType = null,
    string? CompanyImage = null);
