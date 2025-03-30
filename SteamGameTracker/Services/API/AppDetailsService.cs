using Microsoft.Extensions.Logging;
using SteamGameTracker.Components;
using SteamGameTracker.DataTransferObjects.SteamApi.Models;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;

namespace SteamGameTracker.Services.API
{
    public class AppDetailsService : ApiServiceBase, IAppDetailsService
    {
        public AppDetailsService(IUrlFormatter urlFormatter, HttpClient httpClient, ILogger<AppDetailsService> logger) 
            : base(httpClient, logger, urlFormatter)
        {
        }

        public async Task<AppDetailsModel?> GetAppDetailsAsync(int appId, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var url = GetFormattedAppDetailsUrl(appId);
                var appDetailsDTO = await GetDtoAsync<AppDetailsDTO>(url, cancellationToken);

                return appDetailsDTO is not null ? new AppDetailsModel(appDetailsDTO[appId]) : null;
            }
            catch (HttpRequestException ex)
            {
                Log.LogError(ex, "Error fetching app details for app id {appId}", appId);
                throw;
            }
            catch (OperationCanceledException ex)
            {
                Log.LogWarning(ex, "GetAppDetails request for app id '{appId}' was cancelled", appId);
                throw;
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Unexpected error fetching app details for app id {appId}", appId);
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
        }

        private string GetFormattedAppDetailsUrl(params int[] appIds)
        {
            return UrlFormatter.GetFormattedUrl(new GetAppDetailsUrl(appIds));
        }
    }
}
