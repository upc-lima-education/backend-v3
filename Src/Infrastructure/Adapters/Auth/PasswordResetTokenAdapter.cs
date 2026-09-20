using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Domain.Contracts.Auth;
using StackExchange.Redis;

namespace Backend.Src.Infrastructure.Adapters.Auth;

public class PasswordResetTokenAdapter(IConnectionMultiplexer redis) : IPasswordResetTokenPort
{
    private const string KeyPrefix = "password-reset:";
    private static readonly ConcurrentDictionary<string, (PasswordResetData Data, DateTime ExpiresAt)> _memoryResetTokens = new();

    public async Task<string> GenerateAsync(Guid userId, TimeSpan expiration)
    {
        var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        var codeHash = Hash(code);
        var data = new PasswordResetData(userId, false);

        if (redis.IsConnected)
        {
            try
            {
                var db = redis.GetDatabase();
                await db.StringSetAsync(
                    $"{KeyPrefix}{codeHash}",
                    JsonSerializer.Serialize(data),
                    expiration
                );
                return code;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] Error saving password reset token: {ex.Message}. Using in-memory fallback.");
            }
        }

        _memoryResetTokens[$"{KeyPrefix}{codeHash}"] = (data, DateTime.UtcNow.Add(expiration));
        return code;
    }

    public async Task<PasswordResetData?> GetAsync(string code)
    {
        var codeHash = Hash(code);

        if (redis.IsConnected)
        {
            try
            {
                var db = redis.GetDatabase();
                var value = await db.StringGetAsync($"{KeyPrefix}{codeHash}");
                if (!value.IsNullOrEmpty)
                {
                    return JsonSerializer.Deserialize<PasswordResetData>(value.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] Error getting password reset token: {ex.Message}. Using in-memory fallback.");
            }
        }

        if (_memoryResetTokens.TryGetValue($"{KeyPrefix}{codeHash}", out var item))
        {
            if (item.ExpiresAt > DateTime.UtcNow) return item.Data;
            _memoryResetTokens.TryRemove($"{KeyPrefix}{codeHash}", out _);
        }

        return null;
    }

    public async Task MarkAsVerifiedAsync(string code)
    {
        var codeHash = Hash(code);

        if (redis.IsConnected)
        {
            try
            {
                var db = redis.GetDatabase();
                var key = $"{KeyPrefix}{codeHash}";
                var value = await db.StringGetAsync(key);
                if (!value.IsNullOrEmpty)
                {
                    var data = JsonSerializer.Deserialize<PasswordResetData>(value.ToString());
                    if (data is not null)
                    {
                        var verifiedData = new PasswordResetData(data.UserId, true);
                        var ttl = await db.KeyTimeToLiveAsync(key);
                        if (ttl is not null)
                        {
                            await db.StringSetAsync(
                                key,
                                JsonSerializer.Serialize(verifiedData),
                                ttl.Value
                            );
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] Error marking reset token as verified: {ex.Message}. Using in-memory fallback.");
            }
        }

        if (_memoryResetTokens.TryGetValue($"{KeyPrefix}{codeHash}", out var item))
        {
            if (item.ExpiresAt > DateTime.UtcNow)
            {
                _memoryResetTokens[$"{KeyPrefix}{codeHash}"] = (new PasswordResetData(item.Data.UserId, true), item.ExpiresAt);
            }
        }
    }

    public async Task RemoveAsync(string code)
    {
        var codeHash = Hash(code);

        if (redis.IsConnected)
        {
            try
            {
                var db = redis.GetDatabase();
                await db.KeyDeleteAsync($"{KeyPrefix}{codeHash}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Redis] Error removing reset token: {ex.Message}.");
            }
        }

        _memoryResetTokens.TryRemove($"{KeyPrefix}{codeHash}", out _);
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}