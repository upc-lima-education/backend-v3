namespace Backend.Src.Application.Dtos.Data.Profiles;

public record WorkExperienceData(
    string Company,
    string Position,
    string? Description,
    DateOnly StartDate,
    DateOnly? EndDate
);