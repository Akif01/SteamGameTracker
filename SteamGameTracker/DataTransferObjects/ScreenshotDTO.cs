using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class ScreenshotDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("path_thumbnail")]
        public string PathThumbnail { get; set; }

        [JsonPropertyName("path_full")]
        public string PathFull { get; set; }
    }
}
