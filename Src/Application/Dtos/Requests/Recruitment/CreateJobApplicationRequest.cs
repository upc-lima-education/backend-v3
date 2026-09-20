namespace Backend.Src.Application.Dtos.Requests.Recruitment;

public record CreateJobApplicationRequest(
    Guid JobId,
    Stream CvContent,
    string FileName,
    string ContentType
);