using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class RatingsDTO
    {
        [JsonPropertyName("pegi")]
        public RatingDTO Pegi { get; set; }

        [JsonPropertyName("kgrb")]
        public RatingDTO Kgrb { get; set; }

        [JsonPropertyName("csrr")]
        public RatingDTO Csrr { get; set; }

        [JsonPropertyName("crl")]
        public RatingDTO Crl { get; set; }

        [JsonPropertyName("esrb")]
        public RatingDTO Esrb { get; set; }

        [JsonPropertyName("dejus")]
        public RatingDTO Dejus { get; set; }

        [JsonPropertyName("oflc")]
        public RatingDTO Oflc { get; set; }

        [JsonPropertyName("steam_germany")]
        public SteamGermanyRatingDTO SteamGermany { get; set; }
    }
}
