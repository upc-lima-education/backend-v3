namespace Backend.Src.Application.Dtos.Requests.Conversations;

public record CreateConversationRequest(
    Guid JobId,
    List<Guid> UserIds
);