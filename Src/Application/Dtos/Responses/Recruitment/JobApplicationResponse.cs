namespace Backend.Src.Application.Dtos.Responses.Recruitment;

public record CandidateApplicationSummaryData(
    string? FirstName,
    string? LastName,
    string? ProfilePicture,
    string? PhoneNumber,
    List<string>? Skills
);

public record JobApplicationResponse(
    Guid Id,
    Guid CandidateId,
    string Status,
    DateTime CreatedAt,
    CandidateApplicationSummaryData? Candidate = null
);