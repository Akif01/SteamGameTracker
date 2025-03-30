using SteamGameTracker.Models;

namespace SteamGameTracker.Services.API
{
    internal interface IAppListService
    {
        Task<AppsModel?> GetApps(CancellationToken cancellationToken = default);
    }
}