using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class MovieUrlsDTO
    {
        [JsonPropertyName("480")]
        public string Resolution480 { get; set; }

        [JsonPropertyName("max")]
        public string ResolutionMax { get; set; }
    }
}
