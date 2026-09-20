using Backend.Src.Domain.Entities.Conversations;
using Backend.Src.Domain.Repositories.Conversations;
using Backend.Src.Infrastructure.Persistence.MongoDb.Mappers.Conversations;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Backend.Src.Infrastructure.Persistence.MongoDb.Repositories.Conversations;

public class ConversationRepository(MongoDbContext context) : IConversationRepository
{
    public async Task CreateAsync(Conversation conversation)
    {
        var document = ConversationDocumentMapper.ToDocument(conversation);
        await context.Conversations.InsertOneAsync(document);
    }

    public async Task UpdateAsync(Conversation conversation)
    {
        var document = ConversationDocumentMapper.ToDocument(conversation);
        await context.Conversations.ReplaceOneAsync(
            c => c.Id == conversation.Id,
            document
        );
    }

    public async Task<Conversation?> GetByIdAsync(Guid id)
    {
        var document = await context.Conversations
            .Find(c => c.Id == id)
            .FirstOrDefaultAsync();

        if (document is null) return null;

        var conversation = ConversationDocumentMapper.ToDomainEntity(document);
        return conversation;
    }

    public async Task<List<Conversation>> GetConversationListByJobId(Guid jobId)
    {
        var documentList = await context.Conversations
            .Find(c => c.JobId == jobId)
            .ToListAsync();

        var conversationList = new List<Conversation>();
        foreach (var document in documentList)
        {
            var conversation = ConversationDocumentMapper.ToDomainEntity(document);
            conversationList.Add(conversation);
        }

        return conversationList;
    }

    public async Task<List<Conversation>> GetConversationListByUserIdAsync(Guid userId)
    {
        var documentList = await context.Conversations
            .Find(c => c.UserIds.Contains(userId))
            .ToListAsync();

        var conversationList = new List<Conversation>();
        foreach (var document in documentList)
        {
            var conversation = ConversationDocumentMapper.ToDomainEntity(document);
            conversationList.Add(conversation);
        }

        return conversationList;
    }

    public async Task DeleteAsync(Guid id)
    {
        await context.Conversations.DeleteOneAsync(c => c.Id == id);
    }
}