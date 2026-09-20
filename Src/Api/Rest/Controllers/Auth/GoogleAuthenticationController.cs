using System.Net.Mime;
using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Application.Dtos.Responses.Auth;
using Backend.Src.Application.UseCases.Auth;
using Backend.Src.Infrastructure.Options.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backend.Src.Api.Rest.Controllers.Auth;

[ApiController]
[Route("api/v1/auth/google")]
[Produces(MediaTypeNames.Application.Json)]
public class GoogleAuthenticationController(
    ExternalGetAuthenticationUrlUseCase getAuthUrlUseCase,
    ExternalAuthenticationUseCase externalAuthenticationUseCase,
    IOptions<GoogleAuthOptions> options
) : ControllerBase
{
    [HttpGet("url")]
    [AllowAnonymous]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    public ActionResult<string> GetAuthUrl([FromQuery] string? mode = null, [FromQuery] string? userType = null)
    {
        var response = getAuthUrlUseCase.Execute(mode, userType);
        return Ok(response);
    }

    [HttpPost("authenticate")]
    [AllowAnonymous]
    [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthenticationResponse>> Authenticate(ExternalAuthenticationRequest request)
    {
        var response = await externalAuthenticationUseCase.ExecuteAsync(request);
        return Ok(response);
    }

    [HttpGet("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string? state = null)
    {
        var oauthState = string.IsNullOrWhiteSpace(state)
            ? null
            : Backend.Src.Application.Dtos.Data.Auth.AuthenticationStateData.Decode(state);
        if (oauthState is null)
            return BadRequest("Invalid OAuth state.");

        var profileType = Enum.TryParse<Backend.Src.Application.Dtos.Enums.Profiles.ProfileType>(oauthState.ProfileType, true, out var parsedProfile)
            ? parsedProfile
            : (Backend.Src.Application.Dtos.Enums.Profiles.ProfileType?)null;
        var frontendUrl = options.Value.FrontendUrl;
        var separator = frontendUrl.Contains('?') ? "&" : "?";
        var profile = profileType?.ToString();
        var profileQuery = string.IsNullOrWhiteSpace(profile) ? string.Empty : $"&profileType={Uri.EscapeDataString(profile)}";
        return Redirect($"{frontendUrl}{separator}code={Uri.EscapeDataString(code)}&mode={Uri.EscapeDataString(oauthState.Mode)}{profileQuery}");
    }
}
