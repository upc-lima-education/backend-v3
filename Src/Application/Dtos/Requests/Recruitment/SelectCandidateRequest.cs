namespace Backend.Src.Application.Dtos.Requests.Recruitment;

public record SelectCandidateRequest(
    Guid ApplicationId,
    string Message
);