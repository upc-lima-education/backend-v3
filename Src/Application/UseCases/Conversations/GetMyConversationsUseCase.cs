using Backend.Src.Application.Dtos.Responses.Conversations;
using Backend.Src.Application.Mappers.Conversations;
using Backend.Src.Domain.Repositories.Conversations;

namespace Backend.Src.Application.UseCases.Conversations;

public class GetMyConversationsUseCase(IConversationRepository conversationRepository)
{
    public async Task<List<ConversationResponse>> ExecuteAsync(Guid userId)
    {
        var conversations = await conversationRepository.GetConversationListByUserIdAsync(userId);
        var response = conversations.Select(ConversationResponseMapper.ToResponse).ToList();
        return response;
    }
}
