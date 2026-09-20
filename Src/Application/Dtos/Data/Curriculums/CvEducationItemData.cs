namespace Backend.Src.Application.Dtos.Data.Curriculums;

public record CvEducationItemData(
    string Institution,
    string Study,
    string AcademicLevel,
    DateOnly? StartDate,
    DateOnly? EndDate
);