using System.Net.Mime;
using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Application.UseCases.Auth;
using Backend.Src.Infrastructure.Extensions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Auth;

[ApiController]
[Route("api/v1/password")]
[Produces(MediaTypeNames.Application.Json)]
public class PasswordController(
    ForgotPasswordUseCase forgotPasswordUseCase,
    VerifyPasswordResetCodeUseCase verifyPasswordResetCodeUseCase,
    ResetPasswordUseCase resetPasswordUseCase,
    ChangePasswordUseCase changePasswordUseCase
) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("forgot")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var response = await forgotPasswordUseCase.ExecuteAsync(request);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("verify")]
    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyPasswordResetCode([FromBody] VerifyPasswordResetCodeRequest request)
    {
        var response = await verifyPasswordResetCodeUseCase.ExecuteAsync(request);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await resetPasswordUseCase.ExecuteAsync(request);
        return NoContent();
    }

    [Authorize]
    [HttpPost("change")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await changePasswordUseCase.ExecuteAsync(request, selfUserId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("set")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetPassword([FromBody] ChangePasswordRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await changePasswordUseCase.ExecuteAsync(request, selfUserId);
        return NoContent();
    }
}