using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class RecommendationsDTO
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}
