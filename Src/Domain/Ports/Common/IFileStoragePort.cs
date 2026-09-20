using Backend.Src.Domain.Contracts.Common;

namespace Backend.Src.Domain.Ports.Common;

public interface IFileStoragePort
{
    Task<StorageFileResponse> UploadAsync(StorageUploadRequest request);
    Task<Stream> DownloadAsync(string storageKey);
    Task DeleteAsync(string storageKey);
}