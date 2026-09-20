namespace Backend.Src.Domain.Entities.Conversations;

public class ConversationUser
{
    public Guid UserId { get; private set; }
    public Guid ConversationId { get; private set; }

    public ConversationUser() {}

    public ConversationUser(Guid userId, Guid conversationId)
    {
        UserId = userId;
        ConversationId = conversationId;
    }
}
