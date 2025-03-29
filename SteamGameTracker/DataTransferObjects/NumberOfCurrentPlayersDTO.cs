using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class NumberOfCurrentPlayersDTO
    {
        [JsonPropertyName("response")]
        public NumberOfCurrentPlayerResponseDTO Response { get; set; }
    }
}
