using SteamGameTracker.Models;

namespace SteamGameTracker.Services.API
{
    public interface IAppDetailsService
    {
        Task<AppDetailsModel?> GetAppDetailsModelAsync(int appId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AppDetailsModel>?> GetAppDetailsModelsAsync(int[] appIds, CancellationToken cancellationToken = default);
    }
}