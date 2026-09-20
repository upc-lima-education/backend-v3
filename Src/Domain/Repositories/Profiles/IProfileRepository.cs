using Backend.Src.Domain.Entities.Profiles;

namespace Backend.Src.Domain.Repositories.Profiles;

public interface IProfileRepository
{
    Task CreateAsync(Profile profile);
    Task UpdateAsync(Profile profile);
    Task<Profile?> GetByIdAsync(Guid profileId);
    Task<Profile?> GetByIdForUpdateAsync(Guid profileId);
    Task<Profile?> GetByUserIdAsync(Guid userId);
    Task<Profile?> GetByUserIdForUpdateAsync(Guid userId);
    Task DeleteAsync(Guid profileId);
}