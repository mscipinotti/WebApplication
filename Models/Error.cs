using System.Text.Json;
using System.Text.Json.Serialization;
using WebAPP.Infrastructure;

namespace WebAPP.Models
{
    public class Error
    {
        [JsonPropertyName("propertyName")]
        public string PropertyName { get; set; } = string.Empty;

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; set; } = string.Empty;

        [JsonPropertyName("attemptedValue")]
        [JsonConverter(typeof(StringConverter))]
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
        [JsonConverter(typeof(StringConverter))]
        public string PropertyValue { set; get; } = string.Empty;

        [JsonPropertyName("PropertyPath")]
        public string PropertyPath { set; get; } = string.Empty;

    }
}
