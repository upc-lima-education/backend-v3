namespace Backend.Src.Application.Dtos.Data.Curriculums;

public record CvExperienceItemData(
    string Employer,
    string Position,
    DateOnly? StartDate,
    DateOnly? EndDate,
    string Description
);