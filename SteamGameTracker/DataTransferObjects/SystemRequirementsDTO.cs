using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class SystemRequirementsDTO
    {
        [JsonPropertyName("minimum")]
        public string Minimum { get; set; }

        [JsonPropertyName("recommended")]
        public string Recommended { get; set; }
    }
}
