using Backend.Src.Domain.Contracts.Auth;

namespace Backend.Src.Domain.Ports.Auth;

public interface IPasswordResetTokenPort
{
    Task<string> GenerateAsync(Guid userId, TimeSpan expiration);
    Task<PasswordResetData?> GetAsync(string tokenHash);
    Task MarkAsVerifiedAsync(string code);
    Task RemoveAsync(string tokenHash);
}