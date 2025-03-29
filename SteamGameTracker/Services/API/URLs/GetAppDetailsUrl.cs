namespace SteamGameTracker.Services.API.URLs
{
    public class GetAppDetailsUrl : FormattableUrlBase
    {
        private readonly string[] _appIds;

        public GetAppDetailsUrl(string[] appIds)
        {
            _appIds = appIds ?? throw new ArgumentNullException(nameof(appIds));
        }

        public override string ProvideUrlWithPlaceholders() => "https://store.steampowered.com/api/appdetails?appids={appIds}";

        public override Dictionary<string, string> ProvidePlaceHolderValueDict()
        {
            return new Dictionary<string, string> 
            { 
                { "appIds", string.Join(",", _appIds) }, 
            };
        }
    }
}
