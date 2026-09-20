namespace Backend.Src.Domain.Ports.Common;

public interface IPdfRendererPort
{
    Task<Stream> RenderAsync(string html, CancellationToken cancellationToken = default);
}