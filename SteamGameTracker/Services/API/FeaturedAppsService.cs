
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using SteamGameTracker.Components;
using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;
using System.Text.Json;

namespace SteamGameTracker.Services.API
{
    public class FeaturedAppsService : ApiServiceBase, IFeaturedAppsService
    {
        private readonly IDistributedCache _distributedCache;

        public FeaturedAppsService(HttpClient httpClient, 
            ILogger<FeaturedAppsService> logger, 
            IUrlFormatter urlFormatter, 
            IDistributedCache distributedCache) 
            : base(httpClient, logger, urlFormatter)
        {
            _distributedCache = distributedCache;
        }

        public async Task<FeaturedAppsModel?> GetFeaturedAppsAsync(CancellationToken cancellationToken = default)
        {
            string cacheKey = "FeaturedApps";

            // Try to get the value from Redis cache
            string? cachedData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedData))
            {
                try
                {
                    var cachedDto = JsonSerializer.Deserialize<FeaturedAppsDTO>(cachedData);
                    Log.LogInformation("Returning featured apps from Redis cache");

                    if (cachedDto != null)
                    {
                        return new FeaturedAppsModel(cachedDto);
                    }
                }
                catch (JsonException ex)
                {
                    Log.LogError(ex, "Error deserializing cached data for featureed apps");
                    // Remove corrupted cache entry
                    await _distributedCache.RemoveAsync(cacheKey, cancellationToken);
                }
            }

            try
            {
                var url = GetFormattedFeaturedAppsUrl();
                var featuredAppsDTO = await GetDtoAsync<FeaturedAppsDTO>(url, cancellationToken);
                var result = featuredAppsDTO is not null ? new FeaturedAppsModel(featuredAppsDTO) : null;

                if (result is not null)
                {
                    // Set the cache options
                    var cacheOptions = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    // Serialize and store in Redis
                    string serializedData = JsonSerializer.Serialize(featuredAppsDTO);
                    await _distributedCache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

                    Log.LogInformation("Stored featured apps in Redis cache");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                Log.LogError(ex, "Error fetching featured apps");
                throw;
            }
            catch (OperationCanceledException ex)
            {
                Log.LogWarning(ex, "GetFeaturedAppsAsync request was cancelled");
                throw;
            }
        }

        private string GetFormattedFeaturedAppsUrl()
        {
            return UrlFormatter.GetFormattedUrl(new FeaturedAppsUrl());
        }
    }
}
