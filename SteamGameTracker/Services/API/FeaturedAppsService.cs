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
                    Log.LogError(ex, "Error building featured apps model from cache, removing corrupted entry");
                    await _cacheService.RemoveAsync(cacheKey, cancellationToken);
                }
            }

            var url = GetFormattedFeaturedAppsUrl();
            var featuredAppsDTO = await GetDtoAsync<FeaturedAppsDTO>(url, cancellationToken);

            if (featuredAppsDTO is null)
                return null;

            try
            {
                var result = new FeaturedAppsModel(featuredAppsDTO);
                await _cacheService.SetDtoAsync<FeaturedAppsDTO>(cacheKey, featuredAppsDTO, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Error building featured apps model from API response");
                return null;
            }
        }

        private string GetFormattedFeaturedAppsUrl()
        {
            return UrlFormatter.GetFormattedUrl(new FeaturedAppsUrl());
        }
    }
}
