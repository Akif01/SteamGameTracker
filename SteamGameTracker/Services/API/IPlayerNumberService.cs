using SteamGameTracker.Models;

namespace SteamGameTracker.Services.API
{
    internal interface IPlayerNumberService
    {
        Task<NumberOfCurrentPlayersModel?> GetNumberOfCurrentPlayersModelAsync(int appId, CancellationToken cancellationToken = default);
    }
}