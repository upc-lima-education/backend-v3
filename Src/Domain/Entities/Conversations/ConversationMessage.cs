namespace Backend.Src.Domain.Entities.Conversations;

public class ConversationMessage
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ConversationId { get; private set; } 
    public Guid SenderActorId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTime SentAt { get; private set; } = DateTime.UtcNow;

    public ConversationMessage() {}

    internal ConversationMessage(Guid conversationId, Guid senderActorId, string content)
    {
        Id = Guid.NewGuid();
        ConversationId = conversationId;
        SenderActorId = senderActorId;
        Content = content;
        SentAt = DateTime.UtcNow;
    }

    internal static ConversationMessage Restore(
        Guid id,
        Guid conversationId,
        Guid senderActorId,
        string content,
        DateTime sentAt
    )
    {
        return new ConversationMessage
        {
            Id = id,
            ConversationId = conversationId,
            SenderActorId = senderActorId,
            Content = content,
            SentAt = sentAt
        };
    }
}