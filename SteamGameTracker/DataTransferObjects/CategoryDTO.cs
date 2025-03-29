using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class CategoryDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
