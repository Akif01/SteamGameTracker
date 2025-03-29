using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class SupportInfoDTO
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
