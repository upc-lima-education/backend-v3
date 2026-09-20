using System.Net.Mime;
using Backend.Src.Api.Rest.Dtos.Profiles;
using Backend.Src.Api.Rest.Mappers.Profiles;
using Backend.Src.Application.Dtos.Requests.Common;
using Backend.Src.Application.Dtos.Requests.Profiles;
using Backend.Src.Application.Dtos.Responses.Profiles;
using Backend.Src.Application.UseCases.Profiles;
using Backend.Src.Infrastructure.Extensions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Profiles;

[ApiController]
[Route("api/v1/profile")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
public class ProfileController(
    CreateCandidateProfileUseCase createCandidateUseCase,
    CreateOrganizationProfileUseCase createOrganizationUseCase,
    UpdateCandidateProfileUseCase updateCandidateProfileUseCase,
    UpdateCompanyProfileUseCase updateCompanyProfileUseCase,
    GetProfileByProfileIdUseCase getProfileByUserIdUseCase,
    GetMyProfileUseCase getMyProfileUseCase,
    UploadProfilePictureUseCase uploadProfilePictureUseCase,
    VerifyCompanyProfileUseCase verifyCompanyProfileUseCase,
    ValidateRucUseCase validateRucUseCase,
    UpdateCandidateProfileLanguagesUseCase updateCandidateProfileLanguageUseCase,
    UpdateCandidateProfileEducationsUseCase updateCandidateProfileEducationsUseCase,
    UpdateCandidateProfileWorkExperiencesUseCase updateCandidateProfileWorkExperiencesUseCase
) : ControllerBase
{
    [HttpPost("candidate")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEmployee([FromForm] CreateCandidateProfileApiRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var applicationRequest = CreateCandidateProfileRequestMapper.ToApplicationRequest(request);
        var response = await createCandidateUseCase.ExecuteAsync(applicationRequest, selfUserId);
        return CreatedAtAction(nameof(GetProfileById), new { id = response.Id }, response);
    }

    [HttpPost("company")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateOrganization([FromForm] CreateCompanyProfileApiRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var applicationRequest = CreateCompanyProfileRequestMapper.ToApplicationRequest(request);
        var response = await createOrganizationUseCase.ExecuteAsync(applicationRequest, selfUserId);
        return CreatedAtAction(nameof(GetProfileById), new { id = response.Id }, response);
    }

    [HttpPut("candidate")]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCandidateProfile(UpdateCandidateProfileRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await updateCandidateProfileUseCase.ExecuteAsync(request, selfUserId);
        return Ok(response);
    }

    [HttpPut("company")]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCompanyProfile(UpdateCompanyProfileRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await updateCompanyProfileUseCase.ExecuteAsync(request, selfUserId);
        return Ok(response);
    }

    [HttpGet("me")]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile()
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await getMyProfileUseCase.ExecuteAsync(selfUserId);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileById(Guid id)
    {
        var response = await getProfileByUserIdUseCase.ExecuteAsync(id);
        return Ok(response);
    }

    [HttpPatch("upload-photo")]
    public async Task<ActionResult<UploadProfilePictureResponse>> UploadProfilePicture(IFormFile file)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await using var stream = file.OpenReadStream();
        var request = new UploadProfilePictureRequest(new UploadFileRequest(stream, file.FileName, file.ContentType));
        var response = await uploadProfilePictureUseCase.ExecuteAsync(request, selfUserId);
        return Ok(response);
    }

    [HttpPost("{profileId:guid}/verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyCompany(Guid profileId)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await verifyCompanyProfileUseCase.ExecuteAsync(profileId, selfUserId);
        return Ok();
    }

    [HttpPost("ruc/{ruc}/validate")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateRuc(string ruc)
    {
        var request = new ValidateRucRequest(ruc);
        var isValid = await validateRucUseCase.ExecuteAsync(request);
        return Ok(isValid);
    }

    [HttpPatch("language")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateLanguage([FromBody] UpdateCandidateProfileLanguagesRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await updateCandidateProfileLanguageUseCase.ExecuteAsync(request, selfUserId);
        return NoContent();
    }

    [HttpPatch("education")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateEducation([FromBody] UpdateCandidateProfileEducationsRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await updateCandidateProfileEducationsUseCase.ExecuteAsync(request, selfUserId);
        return NoContent();
    }

    [HttpPatch("experience")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateExperience([FromBody] UpdateCandidateProfileWorkExperiencesRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await updateCandidateProfileWorkExperiencesUseCase.ExecuteAsync(request, selfUserId);
        return NoContent();
    }
}
