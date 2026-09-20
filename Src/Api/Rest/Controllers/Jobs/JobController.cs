using System.Net.Mime;
using Backend.Src.Api.Rest.Dtos.Jobs;
using Backend.Src.Api.Rest.Filters.Jobs;
using Backend.Src.Api.Rest.Mappers.Jobs;
using Backend.Src.Application.Dtos.Requests.Jobs;
using Backend.Src.Application.Dtos.Responses.Jobs;
using Backend.Src.Application.UseCases.Jobs;
using Backend.Src.Infrastructure.Extensions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Jobs;

[ApiController]
[Route("api/v1/job")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
public class JobController(
    ClaimJobUseCase claimJobUseCase,
    CreateInternalJobUseCase createInternalJobUseCase,
    UpdateJobUseCase updateJobUseCase,
    PatchJobScheduleUseCase patchJobScheduleUseCase,
    PatchJobSkillsUseCase patchJobSkillsUseCase,
    DeleteJobUseCase deleteJobUseCase,
    GetJobByIdUseCase getJobByIdUseCase,
    GetJobListUseCase getJobListUseCase,
    GetJobSummaryUseCase getJobSummaryUseCase,
    GetJobListByCompanyIdUseCase getJobListByCompanyIdUseCase,
    SyncScrapedJobsUseCase syncScrapedJobsUseCase
) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<JobResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostJob([FromBody] CreateInternalJobRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await createInternalJobUseCase.ExecuteAsync(request, selfUserId);
        return CreatedAtAction(nameof(GetJobById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<JobResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateJob([FromBody] UpdateJobRequest request, Guid id)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await updateJobUseCase.ExecuteAsync(request, id, selfUserId);
        return Ok(response);
    }

    [HttpPatch("{id:guid}/schedule")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchJobSchedule([FromBody] PatchJobScheduleRequest request, Guid id)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await patchJobScheduleUseCase.ExecuteAsync(request, id, selfUserId);
        return NoContent();
    }

    [HttpPatch("{id:guid}/skill")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchJobSkills([FromBody] PatchJobSkillsRequest request, Guid id)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await patchJobSkillsUseCase.ExecuteAsync(request, id, selfUserId);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteJob(Guid id)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await deleteJobUseCase.ExecuteAsync(id, selfUserId);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType<List<JobResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobs()
    {
        var response = await getJobListUseCase.ExecuteAsync();
        return Ok(response);
    }

    [HttpGet("summary")]
    [ProducesResponseType<JobSummaryResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken = default)
    {
        return Ok(await getJobSummaryUseCase.ExecuteAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<JobResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobById(Guid id)
    {
        var response = await getJobByIdUseCase.ExecuteAsync(id);
        return Ok(response);
    }

    [HttpGet("company/{companyId:guid}")]
    [ProducesResponseType<List<JobResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobsByCompanyId(Guid companyId)
    {
        var response = await getJobListByCompanyIdUseCase.ExecuteAsync(companyId);
        return Ok(response);
    }

    [HttpPatch("{id:guid}/claim")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClaimJob(Guid id)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await claimJobUseCase.ExecuteAsync(id, selfUserId);
        return NoContent();
    }

    [HttpPost("sync")]
    [AllowAnonymous]
    [ServiceFilter(typeof(ScraperApiKeyFilter))]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> SyncScrapedJobs([FromBody] List<ScrapedJobApiRequest> jobs)
    {
        if (jobs.Count == 0)
            return BadRequest(new { detail = "Payload cannnot be empty" });
        var applicationRequest = jobs.Select(SyncJobMapper.ToApplicationRequest);
        var response = await syncScrapedJobsUseCase.ExecuteAsync(applicationRequest);
        return Ok(new { status = "success", jobs_synced = response.JobsSynced, jobs_skipped = response.JobsSkipped });
    }
}
