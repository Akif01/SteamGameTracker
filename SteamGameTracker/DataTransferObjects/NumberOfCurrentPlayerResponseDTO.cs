using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class NumberOfCurrentPlayerResponseDTO
    {
        [JsonPropertyName("player_count")]
        public int PlayerCount { get; set; }

        [JsonPropertyName("result")]
        public int Result { get; set; }
    }
}
