using Backend.Src.Domain.Entities.Curriculums;
using Backend.Src.Domain.Repositories.Curriculums;
using Microsoft.EntityFrameworkCore;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Curriculums;

public class CvRepository(AppDbContext context) : ICvRepository
{
    public async Task CreateAsync(Cv cv)
    {
        context.Cvs.Add(cv);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Cv cv)
    {
        context.Cvs.Update(cv);
        await context.SaveChangesAsync();
    }

    public async Task<Cv?> GetByIdAsync(Guid cvId)
    {
        return await context.Cvs
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == cvId);
    }

    public async Task<Cv?> GetByIdForUpdateAsync(Guid cvId)
    {
        return await context.Cvs
            .FirstOrDefaultAsync(c => c.Id == cvId);
    }

    public async Task<IReadOnlyList<Cv>> GetAllByCandidateIdAsync(Guid candidateId)
    {
        return await context.Cvs
            .AsNoTracking()
            .Where(c => c.CandidateId == candidateId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Cv?> GetCurrentByCandidateIdAsync(Guid candidateId)
    {
        return await context.Cvs
            .AsNoTracking()
            .SingleOrDefaultAsync(c =>
                c.CandidateId == candidateId &&
                c.IsCurrent
            );
    }

    public Task<int> CountByCandidateIdAsync(Guid candidateId)
    {
        return context.Cvs
            .CountAsync(c => c.CandidateId == candidateId);
    }

    public async Task DeleteAsync(Guid cvId)
    {
        await context.Cvs
            .Where(c => c.Id == cvId)
            .ExecuteDeleteAsync();
    }
}