using Backend.Src.Application.Dtos.Requests.Auth;
using Backend.Src.Application.UseCases.Auth;
using Backend.Src.Infrastructure.Extensions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Auth;

[ApiController]
[Route("api/v1/account")]
[Authorize]
public class AccountController(UpdateContactUseCase updateContactUseCase) : ControllerBase
{
    [HttpPut("contact")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateContact([FromBody] UpdateContactRequest request)
    {
        await updateContactUseCase.ExecuteAsync(request, ClaimsPrincipalExtension.GetUserId(User));
        return NoContent();
    }
}
