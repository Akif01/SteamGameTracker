using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;
using System.Text.Json;

namespace SteamGameTracker.Services.API
{
    public class PlayerNumberService : ApiServiceBase, IPlayerNumberService
    {
        private readonly IDistributedCache _distributedCache;

        public PlayerNumberService(HttpClient httpClient, 
            ILogger<PlayerNumberService> logger, 
            IUrlFormatter urlFormatter,
            IDistributedCache distributedCache) 
            : base(httpClient, logger, urlFormatter)
        {
            _distributedCache = distributedCache;
        }

        public async Task<NumberOfCurrentPlayersModel?> GetNumberOfCurrentPlayersAsync(int appId, 
            CancellationToken cancellationToken = default)
        {
            string cacheKey = $"NumberOfCurrentPlayers_{appId}";

            // Try to get the value from Redis cache
            string? cachedData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedData))
            {
                try
                {
                    var cachedDto = JsonSerializer.Deserialize<NumberOfCurrentPlayersDTO>(cachedData);
                    Log.LogInformation("Returning number of current players for app id '{appId}' from Redis cache", appId);

                    if (cachedDto != null)
                    {
                        return new NumberOfCurrentPlayersModel(cachedDto);
                    }
                }
                catch (JsonException ex)
                {
                    Log.LogError(ex, "Error deserializing number of current players for appId '{appId}'", appId);
                    // Remove corrupted cache entry
                    await _distributedCache.RemoveAsync(cacheKey, cancellationToken);
                }
            }

            try
            {
                var url = GetFormattedPlayerCountUrl(appId);
                var dto = await GetDtoAsync<NumberOfCurrentPlayersDTO>(url, cancellationToken);
                var result = dto is not null ? new NumberOfCurrentPlayersModel(dto) : null;

                if (result is not null)
                {
                    // Set the cache options
                    var cacheOptions = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    // Serialize and store in Redis
                    string serializedData = JsonSerializer.Serialize(dto);
                    await _distributedCache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

                    Log.LogInformation("Stored number of current players for app '{appId}' in Redis cache", appId);
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
