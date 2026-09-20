using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Src.Domain.Contracts.Auth;
using Backend.Src.Domain.Entities.Auth;
using Backend.Src.Domain.Ports.Auth;
using Backend.Src.Infrastructure.Options.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Src.Infrastructure.Adapters.Auth;

public class JwtAdapter(IOptions<JwtOptions> options) : IJwtPort
{
    private readonly JwtOptions jwt = options.Value;
    private readonly JwtSecurityTokenHandler handler = new();

    public string GenerateAccessToken(User user)
    {
        var secret = jwt.SecretKey;
        var expires = DateTime.UtcNow.AddHours(jwt.AccessExpiresHours);
        Claim[] claims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        ];
        var token = GenerateToken(secret, expires, claims);
        return token;
    }

    public GeneratedRefreshToken GenerateRefreshToken(Guid userId)
    {
        var jti = Guid.NewGuid().ToString();
        var expiration = TimeSpan.FromDays(jwt.RefreshExpiresDays);
        var expires = DateTime.UtcNow.Add(expiration);
        Claim[] claims =
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("type", "refresh"),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        ];
        var token = GenerateToken(jwt.RefreshSecretKey, expires, claims);
        return new GeneratedRefreshToken(token, jti, expiration);
    }

    public RefreshTokenData? ValidateRefreshToken(string token)
    {
        try
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwt.RefreshSecretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = handler.ValidateToken(token, parameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var type = principal.FindFirst("type")?.Value;
            if (type != "refresh") return null;

            var userIdValue = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (!Guid.TryParse(userIdValue, out var userId) ||
                string.IsNullOrEmpty(jti))
            {
                return null;
            }

            return new RefreshTokenData(userId, jti);
        }
        catch
        {
            return null;
        }
    }

    private string GenerateToken(string secret, DateTime expires, IEnumerable<Claim> claims)
    {
        var key = Encoding.ASCII.GetBytes(secret);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = handler.WriteToken(handler.CreateToken(descriptor));
        return token;
    }
}