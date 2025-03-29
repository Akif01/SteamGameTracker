using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class PlatformsDTO
    {
        [JsonPropertyName("windows")]
        public bool Windows { get; set; }

        [JsonPropertyName("mac")]
        public bool Mac { get; set; }

        [JsonPropertyName("linux")]
        public bool Linux { get; set; }
    }
}
