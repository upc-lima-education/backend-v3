using Backend.Src.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Backend.Src.Domain.Repositories.Auth;

namespace Backend.Src.Infrastructure.Persistence.PostgreSql.Repositories.Auth;

public class UserRepository(AppDbContext context) : IUserRepository
{

    public async Task CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        await context.SaveChangesAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByIdForUpdateAsync(Guid id)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var userList = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        return userList;
    }
}