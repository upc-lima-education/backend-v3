using System.Security.Claims;

namespace Backend.Src.Infrastructure.Extensions.Auth;

public static class ClaimsPrincipalExtension
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var id))
        {
            throw new UnauthorizedAccessException("User identity claim is missing or invalid.");
        }

        return id;
    }
}