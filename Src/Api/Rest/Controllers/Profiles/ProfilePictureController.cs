using Backend.Src.Domain.Ports.Common;
using Backend.Src.Domain.Repositories.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Profiles;

[ApiController]
[Route("profiles")]
[AllowAnonymous]
public sealed class ProfilePictureController(
    IProfileRepository profileRepository,
    IFileStoragePort fileStorage
) : ControllerBase
{
    private static readonly FileExtensionContentTypeProvider ContentTypes = new();

    [HttpGet("{id:guid}/profile-picture")]
    [HttpGet("/api/v1/profile/{id:guid}/profile-picture")]
    [HttpGet("/api/v1/profiles/{id:guid}/profile-picture")]
    [HttpGet("/api/v1/profile/{id:guid}/photo")]
    [HttpGet("/api/v1/profiles/{id:guid}/photo")]
    [Produces("image/jpeg", "image/png")]
    public async Task<IActionResult> Get(Guid id)
    {
        var profile = await profileRepository.GetByUserIdAsync(id)
            ?? await profileRepository.GetByIdAsync(id);
        if (profile?.ProfilePicture is null)
            return NotFound();

        if (profile.ProfilePicture.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            profile.ProfilePicture.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return Redirect(profile.ProfilePicture);
        }

        Stream content;
        try
        {
            content = await fileStorage.DownloadAsync(profile.ProfilePicture);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }

        if (!ContentTypes.TryGetContentType(profile.ProfilePicture, out var contentType))
            contentType = "application/octet-stream";

        return File(content, contentType, enableRangeProcessing: true);
    }
}
