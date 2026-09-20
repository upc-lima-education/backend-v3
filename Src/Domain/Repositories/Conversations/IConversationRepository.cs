using Backend.Src.Domain.Entities.Conversations;

namespace Backend.Src.Domain.Repositories.Conversations;

public interface IConversationRepository
{
    Task CreateAsync(Conversation conversation);
    Task UpdateAsync(Conversation conversation);
    Task<Conversation?> GetByIdAsync(Guid id);
    Task<List<Conversation>> GetConversationListByJobId(Guid jobId);
    Task<List<Conversation>> GetConversationListByUserIdAsync(Guid userId);
    Task DeleteAsync(Guid id);
}