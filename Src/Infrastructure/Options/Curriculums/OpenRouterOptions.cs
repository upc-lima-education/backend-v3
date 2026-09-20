namespace Backend.Src.Infrastructure.Options.Curriculums;

public class OpenRouterOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public string HttpReferer { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public int MaxTokens { get; init; } = 0;
    public double Temperature { get; init; } = 0.0;
}