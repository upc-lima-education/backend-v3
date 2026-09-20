using Backend.Src.Domain.Entities.Jobs;
using Backend.Src.Domain.Entities.Profiles;
using Backend.Src.Domain.Exceptions.Skills;
using Backend.Src.Domain.Rules.Skills;

namespace Backend.Src.Domain.Entities.Skills;

public class Skill
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public ICollection<Job> Jobs { get; private set; } = [];
    public ICollection<Profile> Profiles { get; private set; } = [];
    public Skill() {}

    public Skill(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidSkillException("Skill name cannot be empty.");

        if (name.Length > SkillRules.MaxSkillLength)
            throw new InvalidSkillException($"Skill must be at most {SkillRules.MaxSkillLength} characters long.");
            
        Id = Guid.NewGuid();
        Name = name;
        Jobs = [];
        Profiles = [];
    }
}
