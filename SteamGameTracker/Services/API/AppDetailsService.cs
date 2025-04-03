using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using SteamGameTracker.DataTransferObjects.SteamApi.Models;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;
using SteamGameTracker.DataTransferObjects;

namespace SteamGameTracker.Services.API
{
    public class AppDetailsService : ApiServiceBase, IAppDetailsService
    {
        private readonly IDistributedCache _distributedCache;

        public AppDetailsService(IUrlFormatter urlFormatter,
            HttpClient httpClient,
            ILogger<AppDetailsService> logger,
            IDistributedCache distributedCache)
            : base(httpClient, logger, urlFormatter)
        {
            _distributedCache = distributedCache;
        }

        public async Task<AppDetailsModel?> GetAppDetailsAsync(int appId,
            CancellationToken cancellationToken = default)
        {
            string cacheKey = $"AppDetails_{appId}";

            try
            {
                // Try to get the value from Redis cache
                string? cachedData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    Log.LogInformation("Returning app details for app '{appId}' from Redis cache", appId);

                    try
                    {
                        var cachedDto = JsonSerializer.Deserialize<SuccessDTO>(cachedData);
                        if (cachedDto != null)
                        {
                            return new AppDetailsModel(cachedDto);
                        }
                    }
                    catch (JsonException ex)
                    {
                        Log.LogError(ex, "Error deserializing cached data for app '{appId}'", appId);
                        // Remove corrupted cache entry
                        await _distributedCache.RemoveAsync(cacheKey, cancellationToken);
                    }
                }

                var url = GetFormattedAppDetailsUrl(appId);
                var appDetailsDTO = await GetDtoAsync<AppDetailsDTO>(url, cancellationToken);
                var result = appDetailsDTO is not null ? new AppDetailsModel(appDetailsDTO[appId]) : null;

                if (result is not null)
                {
                    // Set the cache options
                    var cacheOptions = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    // Store the DTO in cache, not the model
                    var dtoToCache = appDetailsDTO[appId];
                    string serializedData = JsonSerializer.Serialize(dtoToCache);
                    await _distributedCache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

                    Log.LogInformation("Stored app details for app '{appId}' in Redis cache", appId);
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
            // For multiple items, we can check if they exist in cache and fetch only the missing ones
            List<AppDetailsModel> results = [];
            List<int> missingAppIds = [];

            // Check cache for each app id
            foreach (var appId in appIds)
            {
                string cacheKey = $"AppDetails_{appId}";
                string? cachedData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    try
                    {
                        var cachedDto = JsonSerializer.Deserialize<SuccessDTO>(cachedData);
                        if (cachedDto != null)
                        {
                            var model = new AppDetailsModel(cachedDto);
                            results.Add(model);
                            continue;
                        }
                    }
                    catch (JsonException ex)
                    {
                        Log.LogError(ex, "Error deserializing cached data for app '{appId}'", appId);
                        await _distributedCache.RemoveAsync(cacheKey, cancellationToken);
                    }
                }

                missingAppIds.Add(appId);
            }

            // If all items were in cache, return the results
            if (missingAppIds.Count == 0)
            {
                return results;
            }

            // Fetch missing items
            try
            {
                var url = GetFormattedAppDetailsUrl(missingAppIds.ToArray());
                var appDetailsDTO = await GetDtoAsync<AppDetailsDTO>(url, cancellationToken);

                if (appDetailsDTO is not null)
                {
                    foreach (var detail in appDetailsDTO)
                    {
                        var appId = detail.Key;
                        var dto = detail.Value;
                        var model = new AppDetailsModel(dto);
                        results.Add(model);

                        // Cache the DTO, not the model
                        string cacheKey = $"AppDetails_{appId}";
                        var cacheOptions = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                        string serializedData = JsonSerializer.Serialize(dto);
                        await _distributedCache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

                        Log.LogInformation("Stored app details for app '{appId}' in Redis cache", appId);
                    }
                }

                return results.Count > 0 ? results : null;
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

        // Add this method for other services using ModelBase<T> pattern
        public static T DeserializeModelBase<T, TDto>(string json, JsonSerializerOptions options = null)
            where T : ModelBase<TDto>, new()
            where TDto : class
        {
            var dto = JsonSerializer.Deserialize<TDto>(json, options);
            var constructor = typeof(T).GetConstructor(new[] { typeof(TDto) });

            if (constructor != null && dto != null)
            {
                return (T)constructor.Invoke(new object[] { dto });
            }

            return null;
        }
    }
}