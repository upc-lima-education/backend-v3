using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Src.Infrastructure.Persistence.MongoDb.Documents.Conversations;

public class ConversationDocument
{
    [BsonId]
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ConversationMessageDocument> Messages { get; set; } = [];
    public List<Guid> UserIds { get; set; } = [];
}