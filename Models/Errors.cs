using System.Text.Json.Serialization;

namespace WebAPP.Models
{
    public class Errors
    {
        public bool isValid { get; set; }

        [JsonPropertyName("errors")]
        public List<Error> ErrorList { get; set; } = new();
        
        [JsonPropertyName("ruleSetsExecuted")]
        public List<string> RuleSetsExecuted { get; set; } = new();
    }
}
