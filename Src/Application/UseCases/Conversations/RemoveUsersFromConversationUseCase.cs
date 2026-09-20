using Backend.Src.Application.Dtos.Requests.Conversations;
using Backend.Src.Application.Dtos.Responses.Conversations;
using Backend.Src.Application.Mappers.Conversations;
using Backend.Src.Domain.Exceptions.Conversations;
using Backend.Src.Domain.Repositories.Conversations;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Conversations;

public class RemoveUsersFromConversationUseCase(
    IConversationRepository repository,
    IProfileRepository profileRepository,
    IJobRepository jobRepository
)
{
    public async Task<ConversationResponse> ExecuteAsync(Guid conversationId, RemoveUsersFromConversationRequest request, Guid selfUserId)
    {
        var conversation = await repository.GetByIdAsync(conversationId)
            ?? throw new ConversationNotFoundException(conversationId);

        var isParticipant = conversation.Users.Any(u => u.UserId == selfUserId);
        if (!isParticipant)
        {
            var profile = await profileRepository.GetByUserIdAsync(selfUserId);
            var isJobOwner = false;
            if (profile?.CompanyProfile is not null)
            {
                var job = await jobRepository.GetByIdAsync(conversation.JobId);
                isJobOwner = job is not null && job.CompanyId == profile.Id;
            }

            if (!isJobOwner)
                throw new ConversationAccessDeniedException(conversationId);
        }

        conversation.RemoveUsers(request.UserIds);
        await repository.UpdateAsync(conversation);
        
        var response = ConversationResponseMapper.ToResponse(conversation);
        return response;
    }
}
