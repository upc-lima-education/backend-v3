using Backend.Src.Domain.Entities.Skills;

namespace Backend.Src.Domain.Repositories.Skills;

public interface ISkillRepository
{
    Task SaveAsync(Skill skill);
    Task SaveBatchAsync(IReadOnlyList<Skill> skills);
    Task<IReadOnlyList<Skill>> GetAll();
    Task<Skill?> GetById(Guid id);
    Task<Skill?> GetByName(string name);
    Task<IReadOnlyList<Skill>> GetSkillListByNameList(List<string> name);
    Task<Skill?> GetByIdWithJobs(Guid skillId);
    Task DeleteAsync(Guid id);
}
