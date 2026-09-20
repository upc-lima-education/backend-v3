using Backend.Src.Domain.ValueObjects.Curriculums;

namespace Backend.Src.Application.Dtos.Data.Curriculums;

public record CvSectionResponseData(
    Guid Id,
    string Title,
    int Order,
    List<CvSectionItemResponseData> Items
);