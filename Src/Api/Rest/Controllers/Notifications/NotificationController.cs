using System.Net.Mime;
using Backend.Src.Application.Dtos.Responses.Notifications;
using Backend.Src.Application.UseCases.Notifications;
using Backend.Src.Infrastructure.Extensions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Notifications;

[ApiController]
[Route("api/v1/notifications")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Notifications")]
public class NotificationController(
    GetUserNotificationsUseCase listUseCase) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMine()
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await listUseCase.ExecuteAsync(selfUserId);
        return Ok(response);
    }
}
