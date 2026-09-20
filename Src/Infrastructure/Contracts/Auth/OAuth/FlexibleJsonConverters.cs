using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Src.Infrastructure.Contracts.Auth.OAuth;

public sealed class FlexibleBoolJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.True) return true;
        if (reader.TokenType == JsonTokenType.False) return false;
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            return bool.TryParse(str, out var val) && val;
        }
        return false;
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}

public sealed class FlexibleLongJsonConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number) return reader.GetInt64();
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            return long.TryParse(str, out var val) ? val : 0;
        }
        return 0;
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
