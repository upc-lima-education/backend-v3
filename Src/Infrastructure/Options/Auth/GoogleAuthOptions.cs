namespace Backend.Src.Infrastructure.Options.Auth;

public class GoogleAuthOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string FrontendUrl { get; set; } = string.Empty;
}
