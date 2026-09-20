using Backend.Src.Domain.Entities.Recommendation;
using Backend.Src.Domain.ValueObjects.Recommendation;

namespace Backend.Src.Domain.Repositories.Recommendation;

public interface IJobInteractionRepository
{
    Task CreateAsync(JobInteraction interaction);
    Task<IReadOnlyList<JobInteraction>> GetAllAsync();
    Task<IReadOnlyList<JobInteraction>> GetByCandidateProfileIdAsync(Guid candidateProfileId);
    Task<bool> ExistsAsync(Guid candidateProfileId, Guid jobId, JobInteractionType type);
    Task<bool> ExistsRecentlyAsync(Guid candidateProfileId, Guid jobId, JobInteractionType type, TimeSpan cooldown);
}
