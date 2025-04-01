namespace SteamGameTracker.Services.API.URLs
{
    public class GetNumberOfCurrentPlayersUrl : FormattableUrlBase
    {
        private readonly int _appId;

        public GetNumberOfCurrentPlayersUrl(int appId)
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

        public override string ProvideUrlWithPlaceholders() => "https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid={appId}";
    }
}
