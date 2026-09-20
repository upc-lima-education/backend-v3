using Backend.Src.Domain.ValueObjects.Curriculums;

namespace Backend.Src.Application.Dtos.Data.Curriculums;

public record CvSectionRequestData(
    string Title,
    List<CvSectionItemData> Items
);