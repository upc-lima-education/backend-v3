using Backend.Src.Domain.Contracts.Auth;

namespace Backend.Src.Domain.Ports.Auth;

public interface IExternalAuthenticationPort
{
    string GetAuthorizationUrl(string? state = null);
    Task<string?> GetExchangeCodeAsync(string code);
    Task<ExternalUserIdentity?> GetUserIdentityAsync(string idToken);
}