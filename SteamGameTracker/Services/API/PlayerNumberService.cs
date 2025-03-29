﻿using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.DataTransferObjects.SteamApi.Models;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;

namespace SteamGameTracker.Services.API
{
    public class PlayerNumberService : ApiServiceBase, IPlayerNumberService
    {
        public PlayerNumberService(HttpClient httpClient, ILogger<PlayerNumberService> logger, IUrlFormatter urlFormatter) 
            : base(httpClient, logger, urlFormatter)
        {
        }

        public async Task<NumberOfCurrentPlayersModel?> GetNumberOfCurrentPlayersModelAsync(int appId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = GetFormattedPlayerCountUrl(appId);
                var dto = await GetDtoAsync<NumberOfCurrentPlayersDTO>(url, cancellationToken);

                return dto is not null ? new NumberOfCurrentPlayersModel(dto) : null;
            }
            catch (HttpRequestException ex)
            {
                Log.LogError(ex, "Error fetching player count for app id {appId}", appId);
                throw;
            }
            catch (OperationCanceledException ex)
            {
                Log.LogWarning(ex, "GetNumberOfCurrentPlayersModel request for app id '{appId}' was cancelled", appId);
                throw;
            }
        }

        private string GetFormattedPlayerCountUrl(int appId)
        {
            return UrlFormatter.GetFormattedUrl(new GetNumberOfCurrentPlayersUrl(appId));
        }
    }
}
