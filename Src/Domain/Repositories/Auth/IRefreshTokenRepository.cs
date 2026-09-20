namespace Backend.Src.Domain.Repositories.Auth;

public interface IRefreshTokenRepository
{
    Task SaveAsync(Guid userId, string jti, TimeSpan expiration);
    Task<bool> ExistsAsync(string jti);
    Task DeleteAsync(string jti);
}