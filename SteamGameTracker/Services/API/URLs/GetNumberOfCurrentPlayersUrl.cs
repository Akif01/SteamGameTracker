
namespace SteamGameTracker.Services.API.URLs
{
    public class GetNumberOfCurrentPlayersUrl : FormattableUrlBase
    {
        private readonly string _appId;

        public GetNumberOfCurrentPlayersUrl(string appId)
        {
            _appId = appId ?? throw new ArgumentNullException(appId);
        }

        public override Dictionary<string, string> ProvidePlaceHolderValueDict()
        {
            return new Dictionary<string, string> 
            {
                { "appId", _appId }, 
            };
        }

        public override string ProvideUrlWithPlaceholders() => "https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid={appId}";
    }
}
