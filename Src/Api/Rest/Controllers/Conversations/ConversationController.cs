using Backend.Src.Application.Dtos.Responses.Conversations;
using Backend.Src.Application.Dtos.Requests.Conversations;
using Backend.Src.Application.UseCases.Conversations;
using Backend.Src.Infrastructure.Extensions.Auth;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Src.Api.Rest.Controllers.Conversations;

[ApiController]
[Route("api/v1/conversation")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
public class ConversationController(
    CreateConversationUseCase createConversationUseCase,
    DeleteConversationUseCase deleteConversationUseCase,
    SendMessageUseCase sendMessageUseCase,
    AddUsersToConversationUseCase addUsersUseCase,
    RemoveUsersFromConversationUseCase removeUsersUseCase,
    GetConversationByIdUseCase getConversationByIdUseCase,
    GetConversationListByJobIdUseCase getConversationListByJobIdUseCase,
    GetMyConversationsUseCase getMyConversationsUseCase
) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ConversationResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostConversation([FromBody] CreateConversationRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await createConversationUseCase.ExecuteAsync(request, selfUserId);
        return Ok(response);
    }

    [HttpGet("me")]
    [ProducesResponseType<List<ConversationResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyConversations()
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await getMyConversationsUseCase.ExecuteAsync(selfUserId);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteConversation(Guid id)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await deleteConversationUseCase.ExecuteAsync(id, selfUserId);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ConversationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConversationById(Guid id)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await getConversationByIdUseCase.ExecuteAsync(id, selfUserId);
        return Ok(response);
    }

    [HttpGet("job/{jobId:guid}")]
    [ProducesResponseType<List<ConversationResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConversationsByJobId(Guid jobId)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await getConversationListByJobIdUseCase.ExecuteAsync(jobId, selfUserId);
        return Ok(response);
    }

    [HttpPost("send-message")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        await sendMessageUseCase.ExecuteAsync(request, selfUserId);
        return NoContent();
    }

    [HttpPost("{id:guid}/users")]
    [ProducesResponseType<ConversationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddUsers(Guid id, [FromBody] AddUsersToConversationRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await addUsersUseCase.ExecuteAsync(id, request, selfUserId);
        return Ok(response);
    }

    [HttpDelete("{id:guid}/users")]
    [ProducesResponseType<ConversationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveUsers(Guid id, [FromBody] RemoveUsersFromConversationRequest request)
    {
        var selfUserId = ClaimsPrincipalExtension.GetUserId(User);
        var response = await removeUsersUseCase.ExecuteAsync(id, request, selfUserId);
        return Ok(response);
    }
}
