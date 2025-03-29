using SteamGameTracker.DataTransferObjects.SteamApi.Models;
using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class SuccessDTO
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public GameDataDTO Data { get; set; }
    }
}
