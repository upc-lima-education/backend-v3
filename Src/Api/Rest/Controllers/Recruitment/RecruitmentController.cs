using System.Net.Mime;
using Backend.Src.Api.Rest.Dtos.Recruitment;
using Backend.Src.Api.Rest.Mappers.Recruitment;
using Backend.Src.Application.Dtos.Requests.Recruitment;
using Backend.Src.Application.UseCases.Recruitment;
using Backend.Src.Infrastructure.Extensions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Recruitment;

[ApiController]
[Route("api/v1/recruitment/applications")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
public class RecruitmentController(
    CreateJobApplicationUseCase createApplicationUseCase,
    GetJobApplicationUseCase getJobApplicationUseCase,
    GetJobApplicationsByJobUseCase getJobApplicationsByJobUseCase,
    ApproveJobApplicationUseCase approveApplicationUseCase,
    RejectJobApplicationUseCase rejectApplicationUseCase,
    GetMyJobApplicationsUseCase getMyJobApplicationsUseCase
) : ControllerBase
{
    [HttpPost("send")]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromForm] CreateJobApplicationApiRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var applicationRequest = CreateJobApplicationRequestMapper.ToApplicationRequest(request);
        var id = await createApplicationUseCase.ExecuteAsync(applicationRequest, selfUserId);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyApplications()
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await getMyJobApplicationsUseCase.ExecuteAsync(selfUserId);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await getJobApplicationUseCase.ExecuteAsync(id, selfUserId);
        return File(
            response.Content,
            response.ContentType,
            response.FileName
        );
    }

    [HttpGet("job/{jobId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllByJobId(Guid jobId)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await getJobApplicationsByJobUseCase.ExecuteAsync(jobId, selfUserId);
        return Ok(response);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveJobApplicationRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await approveApplicationUseCase.ExecuteAsync(id, request, selfUserId);
        return Ok(response);
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectJobApplicationRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await rejectApplicationUseCase.ExecuteAsync(id, request, selfUserId);
        return Ok(response);
    }
}
