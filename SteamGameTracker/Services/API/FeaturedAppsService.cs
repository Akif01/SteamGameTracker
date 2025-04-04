using SteamGameTracker.Components;
using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;

namespace SteamGameTracker.Services.API
{
    public class FeaturedAppsService : ApiServiceBase, IFeaturedAppsService
    {
        private readonly ICacheService _cacheService;

        public FeaturedAppsService(HttpClient httpClient, 
            ILogger<FeaturedAppsService> logger, 
            IUrlFormatter urlFormatter,
            ICacheService cacheService) 
            : base(httpClient, logger, urlFormatter)
        {
            _cacheService = cacheService;
        }

        public async Task<FeaturedAppsModel?> GetFeaturedAppsAsync(CancellationToken cancellationToken = default)
        {
            string cacheKey = "FeaturedApps";

            var cachedDto = await _cacheService.GetDtoAsync<FeaturedAppsDTO>(cacheKey, cancellationToken);

            if (cachedDto is not null)
            {
                try
                {
                    return new FeaturedAppsModel(cachedDto);
                }
                catch (Exception ex)
                {
                    Log.LogError(ex, "Error building featured apps model, removing corrupted entry");
                    await _cacheService.RemoveAsync(cacheKey, cancellationToken);
                }
            }

            try
            {
                var url = GetFormattedFeaturedAppsUrl();
                var featuredAppsDTO = await GetDtoAsync<FeaturedAppsDTO>(url, cancellationToken);
                var result = featuredAppsDTO is not null ? new FeaturedAppsModel(featuredAppsDTO) : null;

                if (result is not null)
                {
                    await _cacheService.SetDtoAsync<FeaturedAppsDTO>(cacheKey, featuredAppsDTO, cancellationToken);
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                Log.LogError(ex, "Http error fetching featured apps");
                throw;
            }
            catch (OperationCanceledException ex)
            {
                Log.LogWarning(ex, "GetFeaturedAppsAsync request was cancelled");
                throw;
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Unexpected error fetching featured apps");
                throw;
            }
        }

        private string GetFormattedFeaturedAppsUrl()
        {
            return UrlFormatter.GetFormattedUrl(new FeaturedAppsUrl());
        }
    }
}
