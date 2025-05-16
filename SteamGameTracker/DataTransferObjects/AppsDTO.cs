using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class AppsDTO
    {
        [JsonPropertyName("apps")]
        public List<AppDTO> Apps { get; set; } = [];
    }
}
