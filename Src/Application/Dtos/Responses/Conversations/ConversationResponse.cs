namespace Backend.Src.Application.Dtos.Responses.Conversations;

public record ConversationMessageResponse(
    Guid Id,
    Guid SenderActorId,
    string Content,
    DateTime SentAt
);

public record ConversationResponse(
    Guid Id,
    Guid JobId,
    List<Guid> Users,
    List<ConversationMessageResponse> Messages,
    DateTime CreatedAt
);