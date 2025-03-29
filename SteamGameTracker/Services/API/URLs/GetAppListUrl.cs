
namespace SteamGameTracker.Services.API.URLs
{
    public class GetAppListUrl : FormattableUrlBase
    {
        public override Dictionary<string, string> ProvidePlaceHolderValueDict() => [];

        public override string ProvideUrlWithPlaceholders() => "https://api.steampowered.com/ISteamApps/GetAppList/v2/";
    }
}
