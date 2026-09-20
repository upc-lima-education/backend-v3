using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization.Metadata;

namespace Backend.Src.Infrastructure.Adapters.Curriculums.OpenRouter;

internal static class OpenRouterCvSchemaFactory
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
    };

    public static JsonNode Create<T>()
    {
        var schema = JsonOptions.GetJsonSchemaAsNode(typeof(T));
        MakeStrict(schema);
        return schema;
    }

    private static void MakeStrict(JsonNode? node)
    {
        if (node is not JsonObject obj) return;

        if (obj["properties"] is JsonObject properties)
        {
            obj["additionalProperties"] = false;

            var required = new JsonArray();
            foreach (var property in properties)
                required.Add(property.Key);
            obj["required"] = required;

            foreach (var property in properties)
                MakeStrict(property.Value);
        }

        if (obj.TryGetPropertyValue("items", out var items))
            MakeStrict(items);
    }
}