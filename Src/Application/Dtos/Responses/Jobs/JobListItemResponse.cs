namespace Backend.Src.Application.Dtos.Responses.Jobs;

public record JobListItemResponse(
    Guid Id,
    string CompanyName,
    string? CompanyImage,
    string Title,
    string? Ubigeo,
    string JobType,
    string JobStatus,
    string OriginPage,
    DateTime? ClosesAt
);