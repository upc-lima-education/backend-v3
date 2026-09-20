namespace Backend.Src.Application.Dtos.Requests.Profiles;
public record UpdateEducationRequest(
    Guid Id,
    string Institution,
    string Degree,
    string? FieldOfStudy,
    DateOnly StartDate,
    DateOnly? EndDate
);