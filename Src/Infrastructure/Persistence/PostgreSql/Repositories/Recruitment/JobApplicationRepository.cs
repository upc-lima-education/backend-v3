using Backend.Src.Domain.Entities.Recruitment;
using Backend.Src.Domain.Repositories.Recruitment;
using Microsoft.EntityFrameworkCore;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Recruitment;

public class JobApplicationRepository(AppDbContext context) : IJobApplicationRepository
{
    public async Task CreateAsync(JobApplication application)
    {
        context.JobApplications.Add(application);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(JobApplication application)
    {
        context.JobApplications.Update(application);
        await context.SaveChangesAsync();
    }

    public async Task<JobApplication?> GetByIdAsync(Guid id)
    {
        var jobApplication = await context.JobApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
        return jobApplication;
    }

    public async Task<JobApplication?> GetByIdForUpdateAsync(Guid id)
    {
        var jobApplication = await context.JobApplications
            .FirstOrDefaultAsync(a => a.Id == id);
        return jobApplication;
    }

    public Task<bool> ExistsAsync(Guid jobId, Guid candidateId)
    {
        return context.JobApplications
            .AsNoTracking()
            .AnyAsync(application => application.JobId == jobId && application.CandidateId == candidateId);
    }

    public async Task<IReadOnlyList<JobApplication>> GetByJobIdAsync(Guid jobId)
    {
        var jobApplicationList = await context.JobApplications
            .AsNoTracking()
            .Where(a => a.JobId == jobId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        return jobApplicationList;
    }

    public async Task<IReadOnlyList<JobApplication>> GetByCandidateIdAsync(Guid candidateId)
    {
        var jobApplicationList = await context.JobApplications
            .AsNoTracking()
            .Where(a => a.CandidateId == candidateId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        return jobApplicationList;
    }
}
