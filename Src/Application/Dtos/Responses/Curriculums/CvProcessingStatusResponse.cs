namespace Backend.Src.Application.Dtos.Responses.Curriculums;

public record CvProcessingStatusResponse(
    Guid Id,
    string Status,
    string? Error
);
