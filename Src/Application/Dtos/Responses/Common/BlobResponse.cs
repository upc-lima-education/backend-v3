namespace Backend.Src.Application.Dtos.Responses.Common;

public record BlobResponse(
    Stream Content,
    string ContentType,
    string FileName
);