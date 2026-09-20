namespace Backend.Src.Application.Dtos.Data.Curriculums;

public record CvHeaderData(
    string FullName,
    string Headline,
    string? Email,
    string? Phone,
    string? Location
);