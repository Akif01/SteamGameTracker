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

            try
            {
                var url = GetFormattedAppDetailsUrl(appId);
                var appDetailsDTO = await GetDtoAsync<AppDetailsDTO>(url, cancellationToken);
                var result = appDetailsDTO is not null ? new AppDetailsModel(appDetailsDTO[appId]) : null;

                if (result is not null)
                {
                    await _cacheService.SetDtoAsync<SuccessDTO>(cacheKey, appDetailsDTO[appId], cancellationToken);
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                Log.LogError(ex, "Http error fetching app details for app id '{appId}'", appId);
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

                        string cacheKey = $"AppDetails_{appId}";
                        await _cacheService.SetDtoAsync(cacheKey, dto, cancellationToken);
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
    }
}