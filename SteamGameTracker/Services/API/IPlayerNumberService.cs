using SteamGameTracker.Models;

namespace SteamGameTracker.Services.API
{
    internal interface IPlayerNumberService
    {
        Task<NumberOfCurrentPlayersModel?> GetNumberOfCurrentPlayersAsync(int appId, CancellationToken cancellationToken = default);
    }
}