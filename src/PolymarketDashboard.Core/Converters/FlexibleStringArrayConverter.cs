using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolymarketDashboard.Core.Converters;

/// <summary>
/// Handles Polymarket's dual-format fields that can arrive as either
/// a proper JSON array or a JSON-encoded string containing an array.
/// </summary>
public sealed class FlexibleStringArrayConverter : JsonConverter<string[]?>
{
    public override string[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;

            case JsonTokenType.String:
                // Field is a JSON-encoded string, e.g. "[\"Yes\",\"No\"]"
                var raw = reader.GetString();
                if (string.IsNullOrWhiteSpace(raw)) return null;
                return JsonSerializer.Deserialize<string[]>(raw);

            case JsonTokenType.StartArray:
                var list = new List<string>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    list.Add(reader.GetString() ?? string.Empty);
                return list.ToArray();

            default:
                return null;
        }
    }

    public override void Write(Utf8JsonWriter writer, string[]? value, JsonSerializerOptions options)
    {
        if (value is null) { writer.WriteNullValue(); return; }
        writer.WriteStartArray();
        foreach (var s in value) writer.WriteStringValue(s);
        writer.WriteEndArray();
    }
}
