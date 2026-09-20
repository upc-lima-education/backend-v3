using Backend.Src.Domain.Entities.Skills;
using Backend.Src.Domain.Repositories.Skills;

namespace Backend.Src.Application.Resolvers.Skills;

public class SkillResolver(ISkillRepository skillRepository)
{
    public async Task<List<Skill>> ResolveAsync(IEnumerable<string> names)
    {
        var skillNames = names
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        if (skillNames.Count == 0) return [];

        var existingSkills = await skillRepository.GetSkillListByNameList(skillNames);
        var existingNames = existingSkills.Select(s => s.Name.ToLower()).ToHashSet();

        var result = new List<Skill>(existingSkills);
        var newSkillsToCreate = new List<Skill>();

        foreach (var name in skillNames)
        {
            if (!existingNames.Contains(name))
            {
                var newSkill = new Skill(name);
                newSkillsToCreate.Add(newSkill);
                result.Add(newSkill);
            }
        }

        if (newSkillsToCreate.Count > 0) await skillRepository.SaveBatchAsync(newSkillsToCreate);

        return result;
    }
}