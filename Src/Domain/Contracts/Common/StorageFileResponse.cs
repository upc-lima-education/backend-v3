namespace Backend.Src.Domain.Contracts.Common;

public record StorageFileResponse(
    string StorageKey,
    long Length
);