using System.Text.Json.Serialization;

namespace Backend.Src.Infrastructure.Contracts.Curriculums.OpenRouter;

/// <summary>
/// The response gotten from Open Router <br/>
/// See: https://openrouter.ai/docs/api_reference/overview#responses
/// </summary>
internal sealed record OpenRouterChatResponse(
    [property: JsonPropertyName("choices")]
    IReadOnlyList<OpenRouterChoice>? Choices
);

internal sealed record OpenRouterChoice(
    [property: JsonPropertyName("message")]
    OpenRouterMessage? Message
);