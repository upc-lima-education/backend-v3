using System.Collections.Concurrent;
using Backend.Src.Domain.Repositories.Auth;
using StackExchange.Redis;

namespace Backend.Src.Infrastructure.Persistence.Redis.Repositories.Auth;

public class RefreshTokenRepository(IConnectionMultiplexer redis) : IRefreshTokenRepository
{
    private const string KeyPrefix = "refresh-token:";
    private static readonly ConcurrentDictionary<string, (string UserId, DateTime ExpiresAt)> _memoryTokens = new();

    public async Task SaveAsync(Guid userId, string jti, TimeSpan expiration)
    {
        if (redis.IsConnected)
        {
            try
            {
                var db = redis.GetDatabase();
                await db.StringSetAsync(
                    $"{KeyPrefix}{jti}",
                    userId.ToString(),
                    expiration
                );
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] Error saving refresh token: {ex.Message}. Using in-memory fallback.");
            }
        }

        _memoryTokens[$"{KeyPrefix}{jti}"] = (userId.ToString(), DateTime.UtcNow.Add(expiration));
    }

    public async Task<bool> ExistsAsync(string jti)
    {
        if (redis.IsConnected)
        {
            try
            {
                var db = redis.GetDatabase();
                return await db.KeyExistsAsync($"{KeyPrefix}{jti}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] Error checking refresh token: {ex.Message}. Using in-memory fallback.");
            }
        }

        if (_memoryTokens.TryGetValue($"{KeyPrefix}{jti}", out var item))
        {
            if (item.ExpiresAt > DateTime.UtcNow) return true;
            _memoryTokens.TryRemove($"{KeyPrefix}{jti}", out _);
        }
        return false;
    }

    public async Task DeleteAsync(string jti)
    {
        if (redis.IsConnected)
        {
            try
            {
                var db = redis.GetDatabase();
                await db.KeyDeleteAsync($"{KeyPrefix}{jti}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] Error deleting refresh token: {ex.Message}.");
            }
        }

        _memoryTokens.TryRemove($"{KeyPrefix}{jti}", out _);
    }
}