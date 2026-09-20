using Backend.Src.Domain.Contracts.Common;
using Backend.Src.Domain.Ports.Common;
using Backend.Src.Infrastructure.Options.Common;
using Microsoft.Extensions.Options;

namespace Backend.Src.Infrastructure.Adapters.Common;

/// <summary>
/// Allows blob saving in local disk. <br/>
/// <b>MUST NOT</b> be used in production
/// </summary>
public class LocalDiskStorageAdapter(IOptions<LocalBlobStorageOptions> options) : IFileStoragePort
{
    public async Task<StorageFileResponse> UploadAsync(StorageUploadRequest request)
    {
        var path = Path.Combine(options.Value.BasePath, request.StorageKey);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        request.Content.Position = 0;
        await using var file = File.Create(path);
        await request.Content.CopyToAsync(file);
        return new StorageFileResponse(
            request.StorageKey,
            request.Content.Length
        );
    }

    public Task<Stream> DownloadAsync(string storageKey)
    {
        var path = Path.Combine(options.Value.BasePath, storageKey);
        if (!File.Exists(path)) throw new FileNotFoundException(storageKey);
        Stream stream = File.OpenRead(path);
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storageKey)
    {
        var path = Path.Combine(options.Value.BasePath, storageKey);
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }
}