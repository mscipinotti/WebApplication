using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebAPP.Models
{
    public class Error
    {
        [JsonPropertyName("propertyName")]
        public string PropertyName { get; set; } = string.Empty;

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; } = string.Empty;

        [JsonPropertyName("attemptedValue")]
        [JsonConverter(typeof(ObjectPrimitiveConverter))]
        public string AttemptedValue { get; set; } = string.Empty;

        [JsonPropertyName("customState")]
        public string? CustomState { get; set; }

        [JsonPropertyName("severity")]
        public int Severity { get; set; }

        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; } = string.Empty;
        public FormattedMessagePlaceholderValues? formattedMessagePlaceholderValues { get; set; }
        
    }

    public class FormattedMessagePlaceholderValues
    {
        [JsonPropertyName("PropertyName")]
        public string PropertyName { set; get; } = string.Empty;

        [JsonPropertyName("PropertyValue")]
        [JsonConverter(typeof(ObjectPrimitiveConverter))]
        public string PropertyValue { set; get; } = string.Empty;

        [JsonPropertyName("PropertyPath")]
        public string PropertyPath { set; get; } = string.Empty;

    }

    public class ObjectPrimitiveConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Number when reader.TryGetInt32(out var i) => i.ToString(),
                _ => throw new JsonException()
            };
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, value, value.GetType());
    }
}
