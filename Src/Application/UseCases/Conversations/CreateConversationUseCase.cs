using Backend.Src.Application.Dtos.Requests.Conversations;
using Backend.Src.Application.Dtos.Responses.Conversations;
using Backend.Src.Application.Mappers.Conversations;
using Backend.Src.Domain.Entities.Conversations;
using Backend.Src.Domain.Exceptions.Conversations;
using Backend.Src.Domain.Exceptions.Jobs;
using Backend.Src.Domain.Repositories.Conversations;
using Backend.Src.Domain.Repositories.Jobs;

namespace Backend.Src.Application.UseCases.Conversations;

public class CreateConversationUseCase(
    IConversationRepository conversationRepository,
    IJobRepository jobRepository
)
{
    public async Task<ConversationResponse> ExecuteAsync(CreateConversationRequest request, Guid UserCreatorId)
    {
        var job = await jobRepository.GetByIdAsync(request.JobId)
            ?? throw new JobNotFoundException(request.JobId);
        
        if (!request.UserIds.Any(id => id == UserCreatorId))
            throw new UserNotInConversationException(UserCreatorId);
        
        var conversation = new Conversation(job.Id, request.UserIds);
        await conversationRepository.CreateAsync(conversation);
        
        var response = ConversationResponseMapper.ToResponse(conversation);
        return response;
    }
}
