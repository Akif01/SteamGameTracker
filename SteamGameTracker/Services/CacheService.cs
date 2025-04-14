using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SteamGameTracker.Components;
using System.Text.Json;

namespace SteamGameTracker.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IDistributedCache distributedCache, ILogger<CacheService> logger)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string?> GetStringAsync(string cacheKey, CancellationToken cancellationToken = default)
        {
            return await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
        }

        public async Task<TDto?> GetDtoAsync<TDto>(string cacheKey,
            bool removeCorruptedEntryOnException = true, 
            CancellationToken cancellationToken = default) 
            where TDto : class
        {
            var cachedData = await GetStringAsync(cacheKey, cancellationToken);

            _logger.LogInformation("{GetDtoAsync} - Cache key: '{cacheKey}', Cached data: {cachedData}", 
                nameof(GetDtoAsync), 
                cacheKey, 
                cachedData);

            if (!string.IsNullOrEmpty(cachedData))
            {
                try
                {
                    var cachedDto = JsonSerializer.Deserialize<TDto>(cachedData);
                    _logger.LogInformation("{GetDtoAsync} - Returning dto '{TDto}'", 
                        nameof(GetDtoAsync), 
                        typeof(TDto));

                    return cachedDto;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "{GetDtoAsync} - Error deserializing cached data '{cachedData}' for dto '{TDto}'",
                        nameof(GetDtoAsync), 
                        cachedData, 
                        typeof(TDto));

                    // Remove corrupted cache entry
                    if (removeCorruptedEntryOnException)
                        await RemoveAsync(cacheKey, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{GetDtoAsync} - Fatal error for cache key '{cacheKey}', cache data: '{cachedData}', TDto: '{TDto}'",
                        nameof(GetDtoAsync), 
                        cacheKey,
                        cachedData,
                        typeof(TDto));
                }
            }

            return null;
        }

        public async Task SetDtoAsync<TDto>(string cacheKey, 
            TDto dto,
            CancellationToken cancellationToken = default) where TDto : class
        {
            // Serialize and store in Redis
            string serializedData = JsonSerializer.Serialize(dto);
            var options = GetDefaultCacheEntryOptions();
            await _distributedCache.SetStringAsync(cacheKey, serializedData, options, cancellationToken);

            // Also store the current UTC timestamp as metadata
            string timestampKey = GetTimestampCacheKey(cacheKey);
            string timestampValue = DateTime.UtcNow.ToString("o"); // ISO 8601 format
            await _distributedCache.SetStringAsync(timestampKey, timestampValue, options, cancellationToken);

            _logger. LogInformation("{SetDtoAsync} - Stored cache key '{cacheKey}' with data '{serializedData}' and timestamp '{timestampValue}'", 
                nameof(SetDtoAsync), 
                cacheKey, 
                serializedData, 
                timestampValue);
        }

        public async Task RemoveAsync(string cacheKey,
            CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(cacheKey, cancellationToken);
            _logger.LogInformation("{RemoveAsync} - Stored cache key '{cacheKey}' has been removed",
                nameof(RemoveAsync),
                cacheKey);
        }

        public async Task<DateTime?> GetLastUpdateTimeAsync(string cacheKey, CancellationToken cancellationToken)
        {
            var timestampCacheKey = GetTimestampCacheKey(cacheKey);

            var timestampStr = await _distributedCache.GetStringAsync(timestampCacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(timestampStr) &&
                DateTime.TryParse(timestampStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsedDate))
            {
                return parsedDate;
            }

            return null;
        }

        private string GetTimestampCacheKey(string cacheKey) => $"{cacheKey}_Timestamp";

        private static DistributedCacheEntryOptions GetDefaultCacheEntryOptions() => new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
    }
}
