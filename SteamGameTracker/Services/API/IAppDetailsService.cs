using SteamGameTracker.Models;

namespace SteamGameTracker.Services.API
{
    public interface IAppDetailsService
    {
        Task<AppDetailsModel?> GetAppDetailsAsync(int appId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AppDetailsModel>?> GetAppDetailsAsync(int[] appIds, CancellationToken cancellationToken = default);
    }
}