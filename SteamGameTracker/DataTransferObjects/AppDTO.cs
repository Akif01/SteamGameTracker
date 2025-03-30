using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class AppDTO
    {
        [JsonPropertyName("appid")]
        public int AppId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
