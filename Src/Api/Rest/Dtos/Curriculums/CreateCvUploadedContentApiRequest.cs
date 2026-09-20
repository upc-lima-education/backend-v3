namespace Backend.Src.Api.Rest.Dtos.Curriculums;

public record CreateCvUploadedContentApiRequest(
    string Title,
    bool IsCurrent, //TODO: Handle in a safer way
    IFormFile Cv
);