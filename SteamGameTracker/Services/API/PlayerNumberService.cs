using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.DataTransferObjects.SteamApi.Models;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;

namespace SteamGameTracker.Services.API
{
    public class PlayerNumberService : ApiServiceBase, IPlayerNumberService
    {
        public PlayerNumberService(HttpClient httpClient, ILogger<PlayerNumberService> logger, IUrlFormatter urlFormatter) 
            : base(httpClient, logger, urlFormatter)
        {
        }

        public async Task<NumberOfCurrentPlayersModel?> GetNumberOfCurrentPlayersAsync(int appId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = GetFormattedPlayerCountUrl(appId);
                var dto = await GetDtoAsync<NumberOfCurrentPlayersDTO>(url, cancellationToken);

                return dto is not null ? new NumberOfCurrentPlayersModel(dto) : null;
            }
            catch (HttpRequestException ex)
            {
                Log.LogError(ex, "Error fetching player count for app id {appId}", appId);

                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Log.LogError(ex, "App id {appId} was not found", appId);
                    return null;
                }

                throw;
            }
            catch (OperationCanceledException ex)
            {
                Log.LogWarning(ex, "GetNumberOfCurrentPlayers request for app id '{appId}' was cancelled", appId);
                throw;
            }
        }

        private string GetFormattedPlayerCountUrl(int appId)
        {
            return UrlFormatter.GetFormattedUrl(new GetNumberOfCurrentPlayersUrl(appId));
        }
    }
}
