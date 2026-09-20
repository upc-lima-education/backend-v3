using Backend.Src.Domain.Entities.Jobs;
using Backend.Src.Domain.Repositories.Jobs;
using Microsoft.EntityFrameworkCore;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Jobs;

public class JobRepository(AppDbContext context) : IJobRepository
{
    public async Task CreateAsync(Job job)
    {
        context.Jobs.Add(job);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Job job)
    {
        context.Jobs.Update(job);
        await context.SaveChangesAsync();
    }

    public async Task<Job?> GetByIdAsync(Guid id)
    {
        return await context.Jobs
            .Include(j => j.Company)
                .ThenInclude(c => c!.Profile)
            .Include(j => j.Skills)
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<IReadOnlyList<Job>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return [];

        var now = DateTime.UtcNow;
        return await context.Jobs
            .Where(job => idList.Contains(job.Id) && (job.ClosesAt == null || job.ClosesAt > now))
            .Include(job => job.Company).ThenInclude(company => company!.Profile)
            .Include(job => job.Skills)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Job>> GetRecentActiveJobsAsync(int limit, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await context.Jobs
            .Where(j => (j.ClosesAt == null || j.ClosesAt > now) && j.OpensAt <= now)
            .OrderByDescending(j => j.CreatedAt)
            .Take(limit)
            .Include(j => j.Company).ThenInclude(c => c!.Profile)
            .Include(j => j.Skills)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Job?> GetByIdForUpdateAsync(Guid id)
    {
        return await context.Jobs
            .Include(j => j.Company)
                .ThenInclude(c => c!.Profile)
            .Include(j => j.Skills)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<IReadOnlyList<Job>> GetAllByTitleAsync(string title)
    {
        return await context.Jobs
            .AsNoTracking()
            .Where(j => j.Title == title)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Job>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await context.Jobs
            .AsNoTracking()
            .Where(j => j.CompanyId == companyId)
            .Include(j => j.Company)
                .ThenInclude(c => c!.Profile)
            .ToListAsync();
    }

    public async Task<int> CreateBatchAsync(IEnumerable<Job> jobs)
    {
        await context.Jobs.AddRangeAsync(jobs);
        return await context.SaveChangesAsync();
    }

    public async Task<HashSet<string>> GetExistingSourceUrlsAsync(IEnumerable<string> urls)
    {
        var urlList = urls.ToList();
        var existing = await context.Jobs
            .AsNoTracking()
            .Where(j => j.SourceUrl != null && urlList.Contains(j.SourceUrl))
            .Select(j => j.SourceUrl!)
            .ToListAsync();
        return [.. existing];
    }

    public async Task DeleteAsync(Guid id)
    {
        var job = await context.Jobs.FindAsync(id);
        if (job is null) return;

        context.Jobs.Remove(job);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Job>> GetAllAsync()
    {
        var now = DateTime.UtcNow;
        var jobList = await context.Jobs
            .Where(j => j.ClosesAt == null || j.ClosesAt > now)
            .OrderByDescending(j => j.CreatedAt)
            .Include(j => j.Company)
                .ThenInclude(c => c!.Profile)
            .AsNoTracking()
            .ToListAsync();
        return jobList;
    }

    public Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return context.Jobs
            .AsNoTracking()
            .CountAsync(job => job.ClosesAt == null || job.ClosesAt > now, cancellationToken);
    }
}
