using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class FeaturedAppsDTO
    {
        [JsonPropertyName("large_capsules")]
        public List<FeaturedItemDTO> LargeCapsules { get; set; } = [];

        [JsonPropertyName("featured_win")]
        public List<FeaturedItemDTO> FeaturedWindows { get; set; } = [];

        [JsonPropertyName("featured_mac")]
        public List<FeaturedItemDTO> FeaturedMac { get; set; } = [];

        [JsonPropertyName("featured_linux")]
        public List<FeaturedItemDTO> FeaturedLinux { get; set; } = [];

        [JsonPropertyName("layout")]
        public string? Layout { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}