using SteamGameTracker.Components;
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
            var url = GetFormattedAppListUrl();
            var appListResponstDTO = await GetDtoAsync<AppListResponseDTO>(url, cancellationToken);

            if (appListResponstDTO is null)
                return null;

            try
            {
                var result = new AppsModel(appListResponstDTO.AppList);
                return result;
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Error building apps model from API response");
                return null;
            }
        }

        private string GetFormattedAppListUrl()
        {
            return UrlFormatter.GetFormattedUrl(new GetAppListUrl());
        }
    }
}
