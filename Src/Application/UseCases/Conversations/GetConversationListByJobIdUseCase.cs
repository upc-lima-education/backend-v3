using Backend.Src.Application.Dtos.Responses.Conversations;
using Backend.Src.Application.Mappers.Conversations;
using Backend.Src.Domain.Exceptions.Conversations;
using Backend.Src.Domain.Exceptions.Jobs;
using Backend.Src.Domain.Repositories.Conversations;
using Backend.Src.Domain.Repositories.Jobs;
using Backend.Src.Domain.Repositories.Profiles;

namespace Backend.Src.Application.UseCases.Conversations;

public class GetConversationListByJobIdUseCase(
    IConversationRepository repository,
    IJobRepository jobRepository,
    IProfileRepository profileRepository
)
{
    public async Task<List<ConversationResponse>> ExecuteAsync(Guid jobId, Guid selfUserId)
    {
        var job = await jobRepository.GetByIdAsync(jobId)
            ?? throw new JobNotFoundException(jobId);

        var profile = await profileRepository.GetByUserIdAsync(selfUserId);
        var isJobOwner = profile?.CompanyProfile is not null && job.CompanyId == profile.Id;

        var conversations = await repository.GetConversationListByJobId(jobId);

        if (!isJobOwner)
        {
            conversations = conversations
                .Where(c => c.Users.Any(u => u.UserId == selfUserId))
                .ToList();

            if (conversations.Count == 0)
                throw new ConversationAccessDeniedException(jobId);
        }

        var response = conversations.Select(ConversationResponseMapper.ToResponse).ToList();
        return response;
    }
}
