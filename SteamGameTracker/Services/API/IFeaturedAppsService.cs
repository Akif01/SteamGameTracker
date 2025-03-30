using SteamGameTracker.Models;

namespace SteamGameTracker.Services.API
{
    internal interface IFeaturedAppsService
    {
        Task<FeaturedAppsModel?> GetFeaturedAppsAsync(CancellationToken cancellationToken = default);
    }
}