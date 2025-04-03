using Microsoft.Extensions.Caching.Memory;
using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.DataTransferObjects.SteamApi.Models;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;

namespace SteamGameTracker.Services.API
{
    public class PlayerNumberService : ApiServiceBase, IPlayerNumberService
    {
        private readonly IMemoryCache _memoryCache;

        public PlayerNumberService(HttpClient httpClient, 
            ILogger<PlayerNumberService> logger, 
            IUrlFormatter urlFormatter, 
            IMemoryCache memoryCache) 
            : base(httpClient, logger, urlFormatter)
        {
            _memoryCache = memoryCache;
        }

        public async Task<NumberOfCurrentPlayersModel?> GetNumberOfCurrentPlayersAsync(int appId, 
            CancellationToken cancellationToken = default)
        {
            string cacheKey = $"NumberOfCurrentPlayers_{appId}";

            // Try to get the value from cache
            if (_memoryCache.TryGetValue(cacheKey, out NumberOfCurrentPlayersModel? cachedResult))
            {
                Log.LogInformation("Returning number of current players for app '{appId}' from cache", appId);
                return cachedResult;
            }

            try
            {
                var url = GetFormattedPlayerCountUrl(appId);
                var dto = await GetDtoAsync<NumberOfCurrentPlayersDTO>(url, cancellationToken);
                var result = dto is not null ? new NumberOfCurrentPlayersModel(dto) : null;

                if (result is not null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

                    _memoryCache.Set(cacheKey, result, cacheOptions);
                    Log.LogInformation("Stored number of current players for app '{appId}' in cache", appId);
                }

                return result;
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
