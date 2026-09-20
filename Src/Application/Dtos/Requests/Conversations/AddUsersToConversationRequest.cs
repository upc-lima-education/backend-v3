namespace Backend.Src.Application.Dtos.Requests.Conversations;

public record AddUsersToConversationRequest(
    List<Guid> UserIds
);