namespace Backend.Src.Application.Dtos.Requests.Common;

/// <summary>
/// A generic request for uploading files <br/>
/// To use Fluent Validation, create an especific
/// request and have this record as a parameter
/// </summary>
public record UploadFileRequest(
    Stream Content,
    string FileName,
    string ContentType
);