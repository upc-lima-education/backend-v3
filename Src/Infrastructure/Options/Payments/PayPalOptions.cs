namespace Backend.Src.Infrastructure.Options.Payments;
public class PayPalOptions
{
    public string Mode { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
}
