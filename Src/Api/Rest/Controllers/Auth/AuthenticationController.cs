using System.Net.Mime;
using Backend.Src.Application.Dtos.Data.Auth;
using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Application.Dtos.Responses.Auth;
using Backend.Src.Application.UseCases.Auth;
using Backend.Src.Infrastructure.Extensions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Auth;

[ApiController]
[Route("api/v1/auth")]
[Produces(MediaTypeNames.Application.Json)]
public class AuthenticationController(
    SignInUseCase signInUseCase,
    SignUpUseCase signUpUseCase,
    GetCurrentUserUseCase getCurrentUserUseCase,
    RefreshTokenUseCase refreshTokenUseCase
) : ControllerBase
{
    [HttpPost("sign-up")]
    [AllowAnonymous]
    [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthenticationResponse>> SignUp(SignUpRequest request)
    {
        var response = await signUpUseCase.ExecuteAsync(request);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("sign-in")]
    [AllowAnonymous]
    [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthenticationResponse>> SignIn(SignInRequest request)
    {
        var response = await signInUseCase.ExecuteAsync(request);
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserData>> GetCurrentUser()
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await getCurrentUserUseCase.ExecuteAsync(selfUserId);
        return Ok(response);
    }

    [HttpPost("refresh")]
    [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticationResponse>> Refresh(RefreshTokenRequest request)
    {
        var response = await refreshTokenUseCase.ExecuteAsync(request);
        return Ok(response);
    }
}