namespace SteamGameTracker.Services.API.URLs
{
    public class GetNumberOfCurrentPlayersUrl : FormattableUrlBase
    {
        private readonly int _appId;

        public GetNumberOfCurrentPlayersUrl(int appId)
        {
            _appId = appId;
        }

        public override Dictionary<string, string> ProvidePlaceHolderValueDict()
        {
            return new Dictionary<string, string> 
            {
                { "appId", _appId.ToString() }, 
            };
        }

        public override string ProvideUrlWithPlaceholders() => "https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid={appId}";
    }
}
