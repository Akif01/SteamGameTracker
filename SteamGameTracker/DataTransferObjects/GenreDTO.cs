using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class GenreDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
