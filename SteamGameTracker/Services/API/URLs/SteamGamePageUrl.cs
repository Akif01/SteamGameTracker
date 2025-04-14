
namespace SteamGameTracker.Services.API.URLs
{
    public class SteamGamePageUrl : FormattableUrlBase
    {
        private readonly int _appId;

        public SteamGamePageUrl(int appId)
        {
            _appId = appId;
        }

        public override Dictionary<string, IConvertible> ProvidePlaceHolderValueDict()
        {
            return new Dictionary<string, IConvertible> 
            {
                { "appId", _appId },
            };
        }

        public override string ProvideUrlWithPlaceholders() => "https://store.steampowered.com/app/{appId}/";
    }
}
