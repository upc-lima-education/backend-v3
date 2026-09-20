using System.Security.Claims;
using Backend.Src.Domain.Contracts.Auth;
using Backend.Src.Domain.Entities.Auth;

namespace Backend.Src.Domain.Ports.Auth;

public interface IJwtPort
{
    string GenerateAccessToken(User user);
    GeneratedRefreshToken GenerateRefreshToken(Guid userId);
    RefreshTokenData? ValidateRefreshToken(string token);
}