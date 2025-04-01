namespace SteamGameTracker.Services.API.URLs
{
    public class GetAppDetailsUrl : FormattableUrlBase
    {
        private readonly int[] _appIds;

        public GetAppDetailsUrl(int[] appIds)
        {
            _appIds = appIds ?? throw new ArgumentNullException(nameof(appIds));
        }

        public override string ProvideUrlWithPlaceholders() => "https://store.steampowered.com/api/appdetails?appids={appIds}";

        public override Dictionary<string, IConvertible> ProvidePlaceHolderValueDict()
        {
            return new Dictionary<string, IConvertible> 
            { 
                { "appIds", string.Join(",", _appIds) }, 
            };
        }
    }
}
