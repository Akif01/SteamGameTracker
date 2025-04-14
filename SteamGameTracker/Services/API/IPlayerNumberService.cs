using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.Models;

namespace SteamGameTracker.Services.API
{
    internal interface IPlayerNumberService
    {
        string GetCacheKey(int appId);
        Task<NumberOfCurrentPlayersModel?> GetNumberOfCurrentPlayersAsync(int appId, CancellationToken cancellationToken = default);
        Task<NumberOfCurrentPlayersDTO?> StoreNumberOfCurrentPlayersInCache(int appId, CancellationToken cancellationToken = default);
    }
}