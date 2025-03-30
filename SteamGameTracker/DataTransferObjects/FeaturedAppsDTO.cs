using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class FeaturedAppsDTO
    {
        [JsonPropertyName("large_capsules")]
        public List<FeaturedItemDTO> LargeCapsules { get; set; } = new List<FeaturedItemDTO>();

        [JsonPropertyName("featured_win")]
        public List<FeaturedItemDTO> FeaturedWindows { get; set; } = new List<FeaturedItemDTO>();

        [JsonPropertyName("featured_mac")]
        public List<FeaturedItemDTO> FeaturedMac { get; set; } = new List<FeaturedItemDTO>();

        [JsonPropertyName("featured_linux")]
        public List<FeaturedItemDTO> FeaturedLinux { get; set; } = new List<FeaturedItemDTO>();

        [JsonPropertyName("layout")]
        public string Layout { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}