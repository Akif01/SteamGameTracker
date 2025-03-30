
using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;

namespace SteamGameTracker.Services.API
{
    public class FeaturedAppsService : ApiServiceBase, IFeaturedAppsService
    {
        public FeaturedAppsService(HttpClient httpClient, ILogger<FeaturedAppsService> logger, IUrlFormatter urlFormatter) 
            : base(httpClient, logger, urlFormatter)
        {
        }

        public async Task<FeaturedAppsModel?> GetFeaturedAppsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var url = GetFormattedFeaturedAppsUrl();
                var featuredAppsDTO = await GetDtoAsync<FeaturedAppsDTO>(url, cancellationToken);

                return featuredAppsDTO is not null ? new FeaturedAppsModel(featuredAppsDTO) : null;
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
