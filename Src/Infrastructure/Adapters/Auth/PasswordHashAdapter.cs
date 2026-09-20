using Backend.Src.Domain.Ports.Auth;

namespace Backend.Src.Infrastructure.Adapters.Auth;

public class PasswordHashAdapter : IPasswordHashPort
{
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
