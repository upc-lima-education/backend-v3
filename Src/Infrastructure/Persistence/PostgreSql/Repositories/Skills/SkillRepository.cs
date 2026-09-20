using Backend.Src.Domain.Entities.Skills;
using Backend.Src.Domain.Repositories.Skills;
using Microsoft.EntityFrameworkCore;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Skills;

public class SkillRepository(AppDbContext context) : ISkillRepository
{
    public async Task SaveAsync(Skill skill)
    {
        var existing = await context.Skills.FindAsync(skill.Id);

        if (existing is null) context.Skills.Add(skill);
        else context.Entry(existing).CurrentValues.SetValues(skill);

        await context.SaveChangesAsync();
    }

    public async Task SaveBatchAsync(IReadOnlyList<Skill> skills)
    {
        var skillIds = skills.Select(s => s.Id).ToList();

        var existingSkills = await context.Skills
            .Where(s => skillIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id);
        foreach (var skill in skills)
        {
            if (existingSkills.TryGetValue(skill.Id, out var existing)) context.Entry(existing).CurrentValues.SetValues(skill);
            else context.Skills.Add(skill);
        }

        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Skill>> GetAll()
    {
        return await context.Skills
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Skill?> GetById(Guid id)
    {
        return await context.Skills
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Skill?> GetByName(string name)
    {
        return await context.Skills
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task<IReadOnlyList<Skill>> GetSkillListByNameList(List<string> names)
    {
        return await context.Skills
            .Where(s => names.Contains(s.Name))
            .ToListAsync();
    }

    public async Task<Skill?> GetByIdWithJobs(Guid skillId)
    {
        return await context.Skills
            .Include(s => s.Jobs)
            .FirstOrDefaultAsync(s => s.Id == skillId);
    }

    public async Task DeleteAsync(Guid id)
    {
        var skill = await context.Skills.FindAsync(id);
        if (skill is null) return;

        context.Skills.Remove(skill);
        await context.SaveChangesAsync();
    }
}