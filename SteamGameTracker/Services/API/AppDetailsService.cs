using SteamGameTracker.DataTransferObjects.SteamApi.Models;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;
using SteamGameTracker.DataTransferObjects;

namespace SteamGameTracker.Services.API
{
    public class AppDetailsService : ApiServiceBase, IAppDetailsService
    {
        private readonly ICacheService _cacheService;

        public AppDetailsService(IUrlFormatter urlFormatter,
            HttpClient httpClient,
            ILogger<AppDetailsService> logger,
            ICacheService cacheService)
            : base(httpClient, logger, urlFormatter)
        {
            _cacheService = cacheService;
        }

        public async Task<AppDetailsModel?> GetAppDetailsAsync(int appId,
            CancellationToken cancellationToken = default)
        {
            string cacheKey = $"AppDetails_{appId}";

            var cachedDto = await _cacheService.GetDtoAsync<SuccessDTO>(cacheKey, cancellationToken);

            if (cachedDto is not null)
            {
                try
                {
                    return new AppDetailsModel(cachedDto);
                }
                catch (Exception ex)
                {
                    Log.LogError(ex, "Error building app details model for apppId '{appId}', removing corrupted entry", appId);
                    await _cacheService.RemoveAsync(cacheKey, cancellationToken);
                }
            }

            var url = GetFormattedAppDetailsUrl(appId);
            var appDetailsDTO = await GetDtoAsync<AppDetailsDTO>(url, cancellationToken);

            if (appDetailsDTO is null)
                return null;

            try
            {
                var result = new AppDetailsModel(appDetailsDTO[appId]);
                await _cacheService.SetDtoAsync<SuccessDTO>(cacheKey, appDetailsDTO[appId], cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Error building app details model from API response for app id '{appId}'", appId);
                return null;
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
                string cacheKey = GetCacheKey(appId);
                var cachedDto = await _cacheService.GetDtoAsync<SuccessDTO>(cacheKey, cancellationToken);

                if (cachedDto is not null)
                {
                    try
                    {
                        var model = new AppDetailsModel(cachedDto);
                        results.Add(model);
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Log.LogError(ex, "Error building app details model for apppId '{appId}', removing corrupted entry", appId);
                        await _cacheService.RemoveAsync(cacheKey, cancellationToken);
                    }
                }

                missingAppIds.Add(appId);
            }

            // If all items were in cache, return the results
            if (missingAppIds.Count == 0)
            {
                return results;
            }

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

                    string cacheKey = GetCacheKey(appId);
                    await _cacheService.SetDtoAsync(cacheKey, dto, cancellationToken);
                }
            }

            return results.Count > 0 ? results : null;
        }

        private string GetFormattedAppDetailsUrl(params int[] appIds)
        {
            return UrlFormatter.GetFormattedUrl(new GetAppDetailsUrl(appIds));
        }

        private string GetCacheKey(int appId) => $"AppDetails_{appId}";
    }
}