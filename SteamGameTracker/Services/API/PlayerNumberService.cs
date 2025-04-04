using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;

namespace SteamGameTracker.Services.API
{
    public class PlayerNumberService : ApiServiceBase, IPlayerNumberService
    {
        private readonly ICacheService _cacheService;

        public PlayerNumberService(HttpClient httpClient, 
            ILogger<PlayerNumberService> logger, 
            IUrlFormatter urlFormatter,
            ICacheService cacheService) 
            : base(httpClient, logger, urlFormatter)
        {
            _cacheService = cacheService;
        }

        public async Task<NumberOfCurrentPlayersModel?> GetNumberOfCurrentPlayersAsync(int appId, 
            CancellationToken cancellationToken = default)
        {
            string cacheKey = $"NumberOfCurrentPlayers_{appId}";
            var cachedDto = await _cacheService.GetDtoAsync<NumberOfCurrentPlayersDTO>(cacheKey, cancellationToken);

            if (cachedDto is not null)
            {
                try
                {
                    return new NumberOfCurrentPlayersModel(cachedDto);
                }
                catch (Exception ex)
                {
                    Log.LogError(ex, "Error building model for app id '{appId}' from cache, removing corrupted entry", appId);
                    await _cacheService.RemoveAsync(cacheKey, cancellationToken);
                }
            }

            try
            {
                var url = GetFormattedPlayerCountUrl(appId);
                var dto = await GetDtoAsync<NumberOfCurrentPlayersDTO>(url, cancellationToken);
                var result = dto is not null ? new NumberOfCurrentPlayersModel(dto) : null;

                if (result is not null)
                {
                    await _cacheService.SetDtoAsync<NumberOfCurrentPlayersDTO>(cacheKey, dto, cancellationToken);
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                Log.LogError(ex, "Http error fetching player count for app id '{appId}'", appId);

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
            catch (Exception ex)
            {
                Log.LogError(ex, "Unexpected error fetching number of current players for app id '{appId}'", appId);
                throw;
            }
        }

        private string GetFormattedPlayerCountUrl(int appId)
        {
            return UrlFormatter.GetFormattedUrl(new GetNumberOfCurrentPlayersUrl(appId));
        }
    }
}
