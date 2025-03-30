using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;

namespace SteamGameTracker.Services.API
{
    public class AppListService : ApiServiceBase, IAppListService
    {
        public AppListService(HttpClient httpClient, ILogger<AppListService> logger, IUrlFormatter urlFormatter) 
            : base(httpClient, logger, urlFormatter)
        {
        }

        public async Task<AppsModel?> GetApps(CancellationToken cancellationToken = default)
        {
            try
            {
                var url = GetFormattedAppListUrl();
                var appListResponstDTO = await GetDtoAsync<AppListResponseDTO>(url, cancellationToken);

                return appListResponstDTO is not null ? new AppsModel(appListResponstDTO.AppList) : null;
            }
            catch (HttpRequestException ex)
            {
                Log.LogError(ex, "Error fetching app list");
                throw;
            }
            catch (OperationCanceledException ex)
            {
                Log.LogWarning(ex, "GetRelevantApps request was cancelled");
                throw;
            }
        }

        private string GetFormattedAppListUrl()
        {
            return UrlFormatter.GetFormattedUrl(new GetAppListUrl());
        }
    }
}
