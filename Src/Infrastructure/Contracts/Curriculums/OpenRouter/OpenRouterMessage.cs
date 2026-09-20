using System.Text.Json.Serialization;

namespace Backend.Src.Infrastructure.Contracts.Curriculums.OpenRouter;

internal sealed record OpenRouterMessage(
    [property: JsonPropertyName("role")]
    string Role,
    
    [property: JsonPropertyName("content")]
    string Content
);