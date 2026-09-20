using System.Net.Mime;
using Backend.Src.Application.UseCases.Skills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Skills;

[ApiController]
[Route("api/v1/skill")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
public class SkillController(
    GetSkillListUseCase getSkillListUseCase
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllSkills()
    {
        var response = await getSkillListUseCase.ExecuteAsync();
        return Ok(response);
    }
}