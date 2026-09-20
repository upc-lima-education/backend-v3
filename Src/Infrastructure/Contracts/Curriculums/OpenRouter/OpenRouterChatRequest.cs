using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Backend.Src.Infrastructure.Contracts.Curriculums.OpenRouter;

/// <summary>
/// The prompt that is sent to Open Router API <br/>
/// See: https://openrouter.ai/docs/api_reference/overview#requests
/// </summary>
internal sealed record OpenRouterChatRequest(
    [property: JsonPropertyName("model")]
    string Model,

    [property: JsonPropertyName("messages")]
    IReadOnlyList<OpenRouterMessage> Messages,

    [property: JsonPropertyName("response_format")]
    OpenRouterResponseFormat ResponseFormat,

    [property: JsonPropertyName("temperature")]
    double Temperature,

    [property: JsonPropertyName("max_tokens")]
    int MaxTokens
);

internal sealed record OpenRouterResponseFormat(
    [property: JsonPropertyName("type")]
    string Type,
    
    [property: JsonPropertyName("json_schema")]
    OpenRouterJsonSchema JsonSchema
);

internal sealed record OpenRouterJsonSchema(
    [property: JsonPropertyName("name")]
    string Name,

    [property: JsonPropertyName("strict")]
    bool Strict,

    [property: JsonPropertyName("schema")]
    JsonNode Schema
);