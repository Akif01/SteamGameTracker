using SteamGameTracker.DataTransferObjects.SteamApi.Models;
using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class MovieDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonPropertyName("webm")]
        public MovieUrlsDTO Webm { get; set; }

        [JsonPropertyName("mp4")]
        public MovieUrlsDTO Mp4 { get; set; }

        [JsonPropertyName("highlight")]
        public bool Highlight { get; set; }
    }
}
