using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class ReleaseDateDTO
    {
        [JsonPropertyName("coming_soon")]
        public bool ComingSoon { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }
    }
}
