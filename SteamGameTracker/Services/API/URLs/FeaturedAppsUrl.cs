
namespace SteamGameTracker.Services.API.URLs
{
    public class FeaturedAppsUrl : FormattableUrlBase
    {
        public override Dictionary<string, IConvertible> ProvidePlaceHolderValueDict() => [];

        public override string ProvideUrlWithPlaceholders() => "https://store.steampowered.com/api/featured/";
    }
}
