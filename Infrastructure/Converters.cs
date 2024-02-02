using Microsoft.Extensions.Primitives;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebAPP.Infrastructure
{
    public class Int32Converter : JsonConverter<int>
    {
        // Adattare secondo le necessità
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType switch
            {
                JsonTokenType.String when int.TryParse(reader.GetString(), out int value) => value,
                JsonTokenType.Number => reader.GetInt32(),
                _ => throw new JsonException()
            };

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
    }

    public class StringConverter : JsonConverter<string>
    {
        // Converte (per deserializzazione ad esempio) in string casi di null e interi
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString() ?? string.Empty,
                JsonTokenType.Number when reader.TryGetInt32(out var i) => i.ToString(),
                JsonTokenType.Null => string.Empty,
                _ => throw new JsonException()
            };
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, value, value.GetType());
    }
}
