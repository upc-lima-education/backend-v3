namespace Backend.Src.Infrastructure.Persistence.MongoDb.Documents.Conversations;

public class ConversationMessageDocument
{
    public Guid Id { get; set; }
    public Guid SenderActorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}