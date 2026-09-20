using Backend.Src.Domain.Entities.Curriculums;

namespace Backend.Src.Domain.Repositories.Curriculums;

public interface ICvRepository
{
    Task CreateAsync(Cv cv);
    Task UpdateAsync(Cv cv);
    Task<Cv?> GetByIdAsync(Guid cvId);
    Task<Cv?> GetByIdForUpdateAsync(Guid cvId);
    Task<IReadOnlyList<Cv>> GetAllByCandidateIdAsync(Guid candidateId);
    Task<Cv?> GetCurrentByCandidateIdAsync(Guid candidateId);
    Task<int> CountByCandidateIdAsync(Guid candidateId);
    Task DeleteAsync(Guid cvId);
}