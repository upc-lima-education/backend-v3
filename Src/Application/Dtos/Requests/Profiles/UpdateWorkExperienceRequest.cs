namespace Backend.Src.Application.Dtos.Requests.Profiles;

public record UpdateWorkExperienceRequest(
    Guid Id,
    string Company,
    string Position,
    string? Description,
    DateOnly StartDate,
    DateOnly? EndDate
);