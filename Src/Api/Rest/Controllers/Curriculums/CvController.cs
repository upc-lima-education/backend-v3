using Backend.Src.Api.Rest.Dtos.Curriculums;
using Backend.Src.Api.Rest.Mappers.Curriculums;
using Backend.Src.Application.Dtos.Requests.Curriculums;
using Backend.Src.Application.Dtos.Responses.Curriculums;
using Backend.Src.Application.UseCases.Curriculums;
using Backend.Src.Infrastructure.Extensions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Curriculums;


[ApiController]
[Route("api/v1/cv")]
[Authorize]
[Tags("Curriculums")]
public class CvController(
    CreateCvStructuredContentUseCase createCvStructuredContentUseCase,
    CreateCvUploadedContentUseCase createCvUploadedContentUseCase,
    GetStructuredCvUseCase getStructuredCvUseCase,
    DeleteCvUseCase deleteCvUseCase,
    GetCvUploadedContentUseCase getCvUploadedContentUseCase,
    RequestAiAssistedCvUseCase requestAiAssistedCvUseCase,
    RequestAiAssistedCvImprovementUseCase requestAiAssistedCvImprovementUseCase,
    GenerateCvPdfUseCase generateCvPdfUseCase,
    GetMyCvsUseCase getMyCvsUseCase,
    GetCvProcessingStatusUseCase getCvProcessingStatusUseCase
) : ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType<IReadOnlyList<CvSummaryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyCvs()
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await getMyCvsUseCase.ExecuteAsync(selfUserId);
        return Ok(response);
    }

    [HttpGet("{id:guid}/status")]
    [ProducesResponseType<CvProcessingStatusResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProcessingStatus(Guid id)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        return Ok(await getCvProcessingStatusUseCase.ExecuteAsync(id, selfUserId));
    }

    [HttpPost("structured")]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCvStructuredContent([FromBody] CreateCvStructuredContentRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var id = await createCvStructuredContentUseCase.ExecuteAsync(request, selfUserId);
        return CreatedAtAction(nameof(GetStructuredCv), new { id }, id);
    }

    [HttpPost("uploaded")]
    [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCvUploadedContent([FromForm] CreateCvUploadedContentApiRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var applicationRequest = CreateCvUploadedContentRequestMapper.ToApplicationRequest(request);
        var id = await createCvUploadedContentUseCase.ExecuteAsync(applicationRequest, selfUserId);
        return CreatedAtAction(nameof(GetUploadedCv), new { cvId = id }, id);
    }

    [HttpGet("{id:guid}/structured")]
    [ProducesResponseType<CvStructuredContentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStructuredCv(Guid id)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await getStructuredCvUseCase.ExecuteAsync(id, selfUserId);
        return Ok(response);
    }

    [HttpPost("{id:guid}/transform")]
    [ProducesResponseType<string>(StatusCodes.Status201Created)]

    public async Task<IActionResult> TransformCvStructuredContentToPdf(Guid id)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await generateCvPdfUseCase.ExecuteAsync(id, selfUserId);
        return Ok(response);
    }

    [HttpGet("{cvId}/file")]
    public async Task<IActionResult> GetUploadedCv(Guid cvId)
    {
        var userId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await getCvUploadedContentUseCase.ExecuteAsync(cvId, userId);
        return File(
            response.Content,
            response.ContentType,
            response.FileName
        );
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCv(Guid id)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await deleteCvUseCase.ExecuteAsync(id, selfUserId);
        return Ok();
    }


    [HttpPost("ai-assist-creation")]
    [ProducesResponseType<Guid>(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> GenerateCvAssistedWithAi(AiAssistedCvGenerationRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await requestAiAssistedCvUseCase.ExecuteAsync(request.JobId, selfUserId);
        return Accepted(response);
    }

    [HttpPost("ai-assist-improvement")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> ImproveCvAssistedWithAi(AiAssistedCvImprovementRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await requestAiAssistedCvImprovementUseCase.ExecuteAsync(request, selfUserId);
        return Accepted();
    }
}
