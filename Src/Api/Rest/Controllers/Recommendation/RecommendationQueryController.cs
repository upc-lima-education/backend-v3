using System.Net.Mime;
using Backend.Src.Application.Dtos.Responses.Recommendation;
using Backend.Src.Application.Dtos.Responses.Common;
using Backend.Src.Application.Dtos.Requests.Recommendation;
using Backend.Src.Application.UseCases.Recommendation;
using Backend.Src.Infrastructure.Extensions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Recommendation;

[ApiController]
[Route("api/v1/recommendations")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
public sealed class RecommendationQueryController(
    GetRecommendationsForCandidateUseCase useCase,
    SearchRecommendationsUseCase searchUseCase) : ControllerBase
{
    [HttpGet("for-me")]
    [ProducesResponseType(typeof(IReadOnlyList<RecommendationJobResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetForMe([FromQuery] int limit = 10, CancellationToken cancellationToken = default)
    {
        var userId = ClaimsPrincipalExtension.GetUserId(User);
        return Ok(await useCase.ExecuteAsync(userId, limit, cancellationToken));
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(PagedResponse<RecommendationJobResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Search(
        [FromBody] SearchRecommendationsRequest request,
        CancellationToken cancellationToken = default)
    {
        return Ok(await searchUseCase.ExecuteAsync(request, cancellationToken));
    }
}
