using Backend.Src.Domain.Entities.Recommendation;
using Backend.Src.Domain.Repositories.Recommendation;
using Backend.Src.Domain.ValueObjects.Recommendation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Recommendation;

public sealed class JobInteractionRepository(AppDbContext context) : IJobInteractionRepository
{
    public async Task CreateAsync(JobInteraction interaction)
    {
        context.JobInteractions.Add(interaction);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<JobInteraction>> GetAllAsync() =>
        await context.JobInteractions.AsNoTracking().OrderBy(interaction => interaction.CreatedAt).ToListAsync();

    public async Task<IReadOnlyList<JobInteraction>> GetByCandidateProfileIdAsync(Guid candidateProfileId) =>
        await context.JobInteractions.AsNoTracking()
            .Where(interaction => interaction.CandidateProfileId == candidateProfileId)
            .OrderBy(interaction => interaction.CreatedAt)
            .ToListAsync();

    public Task<bool> ExistsAsync(Guid candidateProfileId, Guid jobId, JobInteractionType type) =>
        context.JobInteractions.AnyAsync(interaction =>
            interaction.CandidateProfileId == candidateProfileId &&
            interaction.JobId == jobId &&
            interaction.Type == type);

    public Task<bool> ExistsRecentlyAsync(Guid candidateProfileId, Guid jobId, JobInteractionType type, TimeSpan cooldown)
    {
        var cutoff = DateTime.UtcNow.Subtract(cooldown);
        return context.JobInteractions.AnyAsync(interaction =>
            interaction.CandidateProfileId == candidateProfileId &&
            interaction.JobId == jobId &&
            interaction.Type == type &&
            interaction.CreatedAt >= cutoff);
    }
}
