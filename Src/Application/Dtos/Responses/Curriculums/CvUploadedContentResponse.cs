namespace Backend.Src.Application.Dtos.Responses.Curriculums;

public record GetCvUploadedResponse(
    Stream Content,
    string ContentType,
    string FileName
);