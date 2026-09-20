using Backend.Src.Domain.Entities.Jobs;

namespace Backend.Src.Domain.Repositories.Jobs;

public interface IJobRepository
{
    Task CreateAsync(Job job);
    Task UpdateAsync(Job job);
    Task<int> CreateBatchAsync(IEnumerable<Job> jobs);
    Task<IReadOnlyList<Job>> GetAllAsync();
    Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);
    Task<Job?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Job>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<IReadOnlyList<Job>> GetRecentActiveJobsAsync(int limit, CancellationToken cancellationToken = default);
    Task<Job?> GetByIdForUpdateAsync(Guid id);
    Task<IReadOnlyList<Job>> GetAllByTitleAsync(string title);
    Task<IReadOnlyList<Job>> GetAllByCompanyIdAsync(Guid companyId);
    Task<HashSet<string>> GetExistingSourceUrlsAsync(IEnumerable<string> urls);
    Task DeleteAsync(Guid id);
}
