namespace Backend.Src.Application.Dtos.Responses.Recruitment;

public record CandidateJobApplicationResponse(
    Guid Id,
    Guid JobId,
    string JobTitle,
    string? CompanyName,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
