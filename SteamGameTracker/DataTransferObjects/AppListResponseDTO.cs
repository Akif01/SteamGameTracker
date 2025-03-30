using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class AppListResponseDTO
    {
        [JsonPropertyName("applist")]
        public AppsDTO AppList { get; set; }
    }
}
