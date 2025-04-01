
using Microsoft.Extensions.Caching.Memory;
using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;

namespace SteamGameTracker.Services.API
{
    public class FeaturedAppsService : ApiServiceBase, IFeaturedAppsService
    {
        private IMemoryCache _memoryCache;

        public FeaturedAppsService(HttpClient httpClient, 
            ILogger<FeaturedAppsService> logger, 
            IUrlFormatter urlFormatter, 
            IMemoryCache memoryCache) 
            : base(httpClient, logger, urlFormatter)
        {
            _memoryCache = memoryCache;
        }

        public async Task<FeaturedAppsModel?> GetFeaturedAppsAsync(CancellationToken cancellationToken = default)
        {
            string cacheKey = "FeaturedApps";

            // Try to get the value from cache
            if (_memoryCache.TryGetValue(cacheKey, out FeaturedAppsModel? cachedResult))
            {
                Log.LogInformation("Returning featured apps from cache");
                return cachedResult;
            }

            try
            {
                var url = GetFormattedFeaturedAppsUrl();
                var featuredAppsDTO = await GetDtoAsync<FeaturedAppsDTO>(url, cancellationToken);
                var result = featuredAppsDTO is not null ? new FeaturedAppsModel(featuredAppsDTO) : null;

                if (result is not null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    // Store in cache
                    _memoryCache.Set(cacheKey, result, cacheOptions);
                    Log.LogInformation("Stored featured apps in cache");
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
