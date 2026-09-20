using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Repositories.Profiles;
using Microsoft.EntityFrameworkCore;


namespace Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Profiles;

public class ProfileRepository(AppDbContext context) : IProfileRepository
{
    public async Task CreateAsync(Profile profile)
    {
        context.Profiles.Add(profile);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Profile profile)
    {
        await context.SaveChangesAsync();
    }

    public async Task<Profile?> GetByIdAsync(Guid profileId)
    {
        return await context.Profiles
            .AsNoTracking()
            .Include(p => p.CandidateProfile)
                .ThenInclude(c => c!.Languages)
            .Include(p => p.CandidateProfile)
                .ThenInclude(c => c!.WorkExperiences)
            .Include(p => p.CandidateProfile)
                .ThenInclude(c => c!.Educations)
            .Include(p => p.CompanyProfile)
            .Include(p => p.Skills)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == profileId);
    }

    public async Task<Profile?> GetByIdForUpdateAsync(Guid profileId)
    {
        return await context.Profiles
            .Include(p => p.CandidateProfile)
                .ThenInclude(c => c!.Languages)
            .Include(p => p.CandidateProfile)
                .ThenInclude(c => c!.WorkExperiences)
            .Include(p => p.CandidateProfile)
                .ThenInclude(c => c!.Educations)
            .Include(p => p.CompanyProfile)
            .Include(p => p.Skills)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == profileId);
    }

    public async Task<Profile?> GetByUserIdAsync(Guid userId)
    {
        return await context.Profiles
            .AsNoTracking()
            .Include(p => p.CandidateProfile)
                .ThenInclude(c => c!.Languages)
            .Include(p => p.CandidateProfile)
                .ThenInclude(c => c!.WorkExperiences)
            .Include(p => p.CandidateProfile)
                .ThenInclude(c => c!.Educations)
            .Include(p => p.CompanyProfile)
            .Include(p => p.Skills)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<Profile?> GetByUserIdForUpdateAsync(Guid userId)
    {
        return await context.Profiles
            .Include(p => p.CandidateProfile)
                .ThenInclude(c => c!.Languages)
            .Include(p => p.CandidateProfile)
                .ThenInclude(c => c!.WorkExperiences)
            .Include(p => p.CandidateProfile)
                .ThenInclude(c => c!.Educations)
            .Include(p => p.CompanyProfile)
            .Include(p => p.Skills)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task DeleteAsync(Guid profileId)
    {
        var profile = await context.Profiles.FindAsync(profileId);
        if (profile == null) return;
        context.Profiles.Remove(profile);
        await context.SaveChangesAsync();
    }
}