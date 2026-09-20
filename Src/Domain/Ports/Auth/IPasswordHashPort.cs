namespace Backend.Src.Domain.Ports.Auth;

public interface IPasswordHashPort
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}