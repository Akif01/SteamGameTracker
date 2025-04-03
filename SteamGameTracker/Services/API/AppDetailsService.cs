using Microsoft.Extensions.Caching.Memory;
using SteamGameTracker.DataTransferObjects.SteamApi.Models;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;

namespace SteamGameTracker.Services.API
{
    public class AppDetailsService : ApiServiceBase, IAppDetailsService
    {
        private readonly IMemoryCache _memoryCache;

        public AppDetailsService(IUrlFormatter urlFormatter, 
            HttpClient httpClient, 
            ILogger<AppDetailsService> logger, 
            IMemoryCache memoryCache) 
            : base(httpClient, logger, urlFormatter)
        {
            _memoryCache = memoryCache;
        }

        public async Task<AppDetailsModel?> GetAppDetailsAsync(int appId, 
            CancellationToken cancellationToken = default)
        {
            string cacheKey = $"AppDetails_{appId}";

            // Try to get the value from cache
            if (_memoryCache.TryGetValue(cacheKey, out AppDetailsModel? cachedResult))
            {
                Log.LogInformation("Returning app details for app '{appId}' from cache", appId);
                return cachedResult;
            }

            try
            {
                var url = GetFormattedAppDetailsUrl(appId);
                var appDetailsDTO = await GetDtoAsync<AppDetailsDTO>(url, cancellationToken);
                var result = appDetailsDTO is not null ? new AppDetailsModel(appDetailsDTO[appId]) : null;

                if (result is not null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _memoryCache.Set(cacheKey, result, cacheOptions);
                    Log.LogInformation("Stored app details for app '{appId}' in cache", appId);
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                Log.LogError(ex, "Error fetching app details for app id '{appId}'", appId);
                throw;
            }
            catch (OperationCanceledException ex)
            {
                Log.LogWarning(ex, "GetAppDetails request for app id '{appId}' was cancelled", appId);
                throw;
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Unexpected error fetching app details for app id '{appId}'", appId);
                throw;
            }
        }

        public async Task<IEnumerable<AppDetailsModel>?> GetAppDetailsAsync(int[] appIds, 
            CancellationToken cancellationToken = default) 
        {
            try
            {
                var url = GetFormattedAppDetailsUrl(appIds);
                var appDetailsDTO = await GetDtoAsync<AppDetailsDTO>(url, cancellationToken);

                return appDetailsDTO is not null ? appDetailsDTO.Select(x => new AppDetailsModel(x.Value)) : null;
            }
            catch (HttpRequestException ex)
            {
                Log.LogError(ex, "Error fetching app details for app ids '{appIds}'", string.Join(",", appIds));
                throw;
            }
            catch (OperationCanceledException ex)
            {
                Log.LogWarning(ex, "GetAppDetails request for app ids '{appIds}' was cancelled", string.Join(",", appIds));
                throw;
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Unexpected error fetching app details for app id '{appIds}'", string.Join(",", appIds));
                throw;
            }
        }

        private string GetFormattedAppDetailsUrl(params int[] appIds)
        {
            return UrlFormatter.GetFormattedUrl(new GetAppDetailsUrl(appIds));
        }
    }
}
