using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class RatingDTO
    {
        [JsonPropertyName("rating")]
        public string Rating { get; set; }

        [JsonPropertyName("descriptors")]
        public string Descriptors { get; set; }
    }
}
