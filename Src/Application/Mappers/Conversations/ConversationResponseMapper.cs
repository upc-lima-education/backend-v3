using Backend.Src.Application.Dtos.Responses.Conversations;
using Backend.Src.Domain.Entities.Conversations;

namespace Backend.Src.Application.Mappers.Conversations;

public static class ConversationResponseMapper
{
    public static ConversationResponse ToResponse(Conversation conversation)
    {
        return new ConversationResponse(
            conversation.Id,
            conversation.JobId,
            conversation.Users.Select(u => u.UserId).ToList(),
            conversation.Messages
                .OrderBy(m => m.SentAt)
                .Select(m => new ConversationMessageResponse(m.Id, m.SenderActorId, m.Content, m.SentAt))
                .ToList(),
            conversation.CreatedAt
        );
    }
}