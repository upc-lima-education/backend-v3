using Backend.Src.Domain.Entities.Auth;

namespace Backend.Src.Domain.Repositories.Auth;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByIdForUpdateAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
}
