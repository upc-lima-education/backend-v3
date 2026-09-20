using Backend.Src.Domain.Entities.Conversations;
using Backend.Src.Infrastructure.Persistence.MongoDb.Documents.Conversations;

namespace Backend.Src.Infrastructure.Persistence.MongoDb.Mappers.Conversations;

public static class ConversationDocumentMapper
{
    public static ConversationDocument ToDocument(Conversation conversation)
    {
        var document = new ConversationDocument
        {
            Id = conversation.Id,
            JobId = conversation.JobId,
            CreatedAt = conversation.CreatedAt,
            UserIds = conversation.Users.Select(u => u.UserId).ToList(),
            Messages = conversation.Messages.Select(
                m => new ConversationMessageDocument
                    {
                        Id = m.Id,
                        SenderActorId = m.SenderActorId,
                        Content = m.Content,
                        SentAt = m.SentAt
                    }
                ).ToList()
        };
        return document;
    }

    public static Conversation ToDomainEntity(ConversationDocument document)
    {
        var conversation = Conversation.Restore(
            document.Id,
            document.JobId,
            document.CreatedAt,
            document.UserIds,
            document.Messages.Select(m =>
                ConversationMessage.Restore(
                    m.Id,
                    document.Id,
                    m.SenderActorId,
                    m.Content,
                    m.SentAt
                )
            )
        );
        return conversation;
    }
}