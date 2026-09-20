namespace Backend.Src.Application.Dtos.Data.Profiles;

public record EducationData(
    string Institution,
    string Degree,
    string? FieldOfStudy,
    DateOnly StartDate,
    DateOnly? EndDate
);