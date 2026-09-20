namespace Backend.Src.Api.Rest.Dtos.Recruitment;

public record CreateJobApplicationApiRequest(
    Guid JobId,
    IFormFile Cv
);