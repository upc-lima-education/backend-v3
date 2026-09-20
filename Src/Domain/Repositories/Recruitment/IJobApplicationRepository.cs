using Backend.Src.Domain.Entities.Recruitment;

namespace Backend.Src.Domain.Repositories.Recruitment;

public interface IJobApplicationRepository
{
    Task CreateAsync(JobApplication application);
    Task UpdateAsync(JobApplication application);
    Task<JobApplication?> GetByIdAsync(Guid id);
    Task<JobApplication?> GetByIdForUpdateAsync(Guid id);
    Task<bool> ExistsAsync(Guid jobId, Guid candidateId);
    Task<IReadOnlyList<JobApplication>> GetByJobIdAsync(Guid jobId);
    Task<IReadOnlyList<JobApplication>> GetByCandidateIdAsync(Guid candidateId);
}
