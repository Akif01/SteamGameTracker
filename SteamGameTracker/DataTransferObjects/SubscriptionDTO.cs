using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class SubscriptionDTO
    {
        [JsonPropertyName("packageid")]
        public int PackageId { get; set; }

        [JsonPropertyName("percent_savings_text")]
        public string PercentSavingsText { get; set; }

        [JsonPropertyName("percent_savings")]
        public int PercentSavings { get; set; }

        [JsonPropertyName("option_text")]
        public string OptionText { get; set; }

        [JsonPropertyName("option_description")]
        public string OptionDescription { get; set; }

        [JsonPropertyName("can_get_free_license")]
        public string CanGetFreeLicense { get; set; }

        [JsonPropertyName("is_free_license")]
        public bool IsFreeLicense { get; set; }

        [JsonPropertyName("price_in_cents_with_discount")]
        public int PriceInCentsWithDiscount { get; set; }
    }
}
