using System.Net.Mime;
using Backend.Src.Application.Dtos.Requests.Recommendation;
using Backend.Src.Application.UseCases.Recommendation;
using Backend.Src.Infrastructure.Extensions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Recommendation;

[ApiController]
[Route("api/v1/job-interactions")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
public sealed class RecommendationController(CreateJobInteractionUseCase createInteractionUseCase) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateJobInteractionRequest request)
    {
        var userId = ClaimsPrincipalExtension.GetUserId(User);
        await createInteractionUseCase.ExecuteAsync(request, userId);
        return NoContent();
    }
}
