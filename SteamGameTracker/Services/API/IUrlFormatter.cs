using SteamGameTracker.Services.API.URLs;

namespace SteamGameTracker.Services.API
{
    public interface IUrlFormatter
    {
        public string GetFormattedUrl(FormattableUrlBase formattableUrl);
    }
}