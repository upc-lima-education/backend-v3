namespace Backend.Src.Domain.Contracts.Common;

public record StorageUploadRequest(
    string StorageKey,
    Stream Content,
    string ContentType
);