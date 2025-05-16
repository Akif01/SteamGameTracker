using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class FeaturedItemDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("discounted")]
        public bool Discounted { get; set; }

        [JsonPropertyName("discount_percent")]
        public int DiscountPercent { get; set; }

        [JsonPropertyName("original_price")]
        public int? OriginalPrice { get; set; }

        [JsonPropertyName("final_price")]
        public int FinalPrice { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("large_capsule_image")]
        public string? LargeCapsuleImage { get; set; }

        [JsonPropertyName("small_capsule_image")]
        public string? SmallCapsuleImage { get; set; }

        [JsonPropertyName("windows_available")]
        public bool WindowsAvailable { get; set; }

        [JsonPropertyName("mac_available")]
        public bool MacAvailable { get; set; }

        [JsonPropertyName("linux_available")]
        public bool LinuxAvailable { get; set; }

        [JsonPropertyName("streamingvideo_available")]
        public bool StreamingVideoAvailable { get; set; }

        [JsonPropertyName("discount_expiration")]
        public long? DiscountExpiration { get; set; }

        [JsonPropertyName("header_image")]
        public string? HeaderImage { get; set; }

        [JsonPropertyName("controller_support")]
        public string? ControllerSupport { get; set; }
    }
}
