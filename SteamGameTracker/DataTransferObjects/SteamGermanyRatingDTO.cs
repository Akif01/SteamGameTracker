using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class SteamGermanyRatingDTO
    {
        [JsonPropertyName("rating_generated")]
        public string RatingGenerated { get; set; }

        [JsonPropertyName("rating")]
        public string Rating { get; set; }

        [JsonPropertyName("required_age")]
        public string RequiredAge { get; set; }

        [JsonPropertyName("banned")]
        public string Banned { get; set; }

        [JsonPropertyName("use_age_gate")]
        public string UseAgeGate { get; set; }

        [JsonPropertyName("descriptors")]
        public string Descriptors { get; set; }
    }
}
