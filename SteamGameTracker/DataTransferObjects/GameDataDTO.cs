using SteamGameTracker.DataTransferObjects.SteamApi.Models;
using System.Text.Json.Serialization;

namespace SteamGameTracker.DataTransferObjects
{
    public class GameDataDTO
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("steam_appid")]
        public int SteamAppId { get; set; }

        [JsonPropertyName("required_age")]
        public int RequiredAge { get; set; }

        [JsonPropertyName("is_free")]
        public bool IsFree { get; set; }

        [JsonPropertyName("detailed_description")]
        public string DetailedDescription { get; set; }

        [JsonPropertyName("about_the_game")]
        public string AboutTheGame { get; set; }

        [JsonPropertyName("short_description")]
        public string ShortDescription { get; set; }

        [JsonPropertyName("supported_languages")]
        public string SupportedLanguages { get; set; }

        [JsonPropertyName("header_image")]
        public string HeaderImage { get; set; }

        [JsonPropertyName("capsule_image")]
        public string CapsuleImage { get; set; }

        [JsonPropertyName("capsule_imagev5")]
        public string CapsuleImageV5 { get; set; }

        [JsonPropertyName("website")]
        public string Website { get; set; }

        [JsonPropertyName("pc_requirements")]
        public SystemRequirementsDTO? PcRequirements { get; set; }

        //[JsonPropertyName("mac_requirements")]
        //public SystemRequirementsDTO? MacRequirements { get; set; }

        //[JsonPropertyName("linux_requirements")]
        //public SystemRequirementsDTO? LinuxRequirements { get; set; }

        [JsonPropertyName("developers")]
        public List<string> Developers { get; set; }

        [JsonPropertyName("publishers")]
        public List<string> Publishers { get; set; }

        [JsonPropertyName("price_overview")]
        public PriceOverviewDTO PriceOverview { get; set; }

        [JsonPropertyName("packages")]
        public List<int> Packages { get; set; }

        [JsonPropertyName("package_groups")]
        public List<PackageGroupDTO> PackageGroups { get; set; }

        [JsonPropertyName("platforms")]
        public PlatformsDTO Platforms { get; set; }

        [JsonPropertyName("categories")]
        public List<CategoryDTO> Categories { get; set; }

        [JsonPropertyName("genres")]
        public List<GenreDTO> Genres { get; set; }

        [JsonPropertyName("screenshots")]
        public List<ScreenshotDTO> Screenshots { get; set; }

        [JsonPropertyName("movies")]
        public List<MovieDTO> Movies { get; set; }

        [JsonPropertyName("recommendations")]
        public RecommendationsDTO Recommendations { get; set; }

        [JsonPropertyName("release_date")]
        public ReleaseDateDTO ReleaseDate { get; set; }

        [JsonPropertyName("support_info")]
        public SupportInfoDTO SupportInfo { get; set; }

        [JsonPropertyName("background")]
        public string Background { get; set; }

        [JsonPropertyName("background_raw")]
        public string BackgroundRaw { get; set; }

        [JsonPropertyName("content_descriptors")]
        public ContentDescriptorsDTO ContentDescriptors { get; set; }

        [JsonPropertyName("ratings")]
        public RatingsDTO Ratings { get; set; }
    }
}
