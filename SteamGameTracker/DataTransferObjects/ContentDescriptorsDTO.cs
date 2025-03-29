using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class ContentDescriptorsDTO
    {
        [JsonPropertyName("ids")]
        public List<int> Ids { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }
    }
}
