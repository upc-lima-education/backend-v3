namespace Backend.Src.Application.Dtos.Responses.Curriculums;

public record CvSummaryResponse(
    Guid Id,
    string Title,
    bool IsCurrent,
    bool HasFileContent,
    string ProcessingStatus,
    string? ProcessingError,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
