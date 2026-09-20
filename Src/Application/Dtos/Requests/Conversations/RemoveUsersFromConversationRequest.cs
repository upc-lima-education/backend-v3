namespace Backend.Src.Application.Dtos.Requests.Conversations;

public record RemoveUsersFromConversationRequest(
    List<Guid> UserIds
);