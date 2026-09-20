namespace Backend.Src.Application.Dtos.Data.Curriculums;

public record CvSectionItemResponseData(
    Guid Id,
    string? Title,
    string Description,
    int Order
);