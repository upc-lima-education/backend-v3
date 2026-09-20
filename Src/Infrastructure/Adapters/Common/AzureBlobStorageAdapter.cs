using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Backend.Src.Domain.Contracts.Common;
using Backend.Src.Domain.Ports.Common;

namespace Backend.Src.Infrastructure.Adapters.Common;

public class AzureBlobStorageAdapter(BlobContainerClient container) : IFileStoragePort
{
    public async Task<StorageFileResponse> UploadAsync(StorageUploadRequest request)
    {
        await container.CreateIfNotExistsAsync(PublicAccessType.None);
        if (request.Content.CanSeek)
            request.Content.Position = 0;

        var blob = container.GetBlobClient(request.StorageKey);
        await blob.UploadAsync(
            request.Content,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = request.ContentType
                }
            }
        );
        var response = new StorageFileResponse(request.StorageKey, request.Content.Length);
        return response;
    }

    public async Task<Stream> DownloadAsync(string storageKey)
    {
        var blob = container.GetBlobClient(storageKey);
        if (!await blob.ExistsAsync()) throw new FileNotFoundException(storageKey);

        var download = await blob.DownloadStreamingAsync();
        return download.Value.Content;
    }

    public async Task DeleteAsync(string storageKey)
    {
        var blob = container.GetBlobClient(storageKey);
        await blob.DeleteIfExistsAsync();
    }
}
