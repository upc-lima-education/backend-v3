namespace Backend.Src.Application.Dtos.Requests.Conversations;

public record SendMessageRequest(
    Guid ConversationId,
    string Content
);