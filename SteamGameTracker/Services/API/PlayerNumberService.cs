using SteamGameTracker.DataTransferObjects;
using SteamGameTracker.Models;
using SteamGameTracker.Services.API.URLs;

namespace SteamGameTracker.Services.API
{
    public class PlayerNumberService : ApiServiceBase, IPlayerNumberService
    {
        private readonly ICacheService _cacheService;

        public PlayerNumberService(HttpClient httpClient, 
            ILogger<PlayerNumberService> logger, 
            IUrlFormatter urlFormatter,
            ICacheService cacheService) 
            : base(httpClient, logger, urlFormatter)
        {
            _cacheService = cacheService;
        }

        public async Task<NumberOfCurrentPlayersModel?> GetNumberOfCurrentPlayersAsync(int appId,
            CancellationToken cancellationToken = default)
        {
            string cacheKey = GetCacheKey(appId);
            var cachedDto = await _cacheService.GetDtoAsync<NumberOfCurrentPlayersDTO>(cacheKey, cancellationToken: cancellationToken);

            if (cachedDto is not null)
            {
                try
                {
                    return new NumberOfCurrentPlayersModel(cachedDto);
                }
                catch (Exception ex)
                {
                    Log.LogError(ex, "Error building model for app id '{appId}' from cache, removing corrupted entry", appId);
                    await _cacheService.RemoveAsync(cacheKey, cancellationToken);
                }
            }

            var dto = await StoreNumberOfCurrentPlayersInCache(appId, cancellationToken);

            if (dto is null)
                return null;

            try
            {
                var result = new NumberOfCurrentPlayersModel(dto);

                return result;
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Error building player count model from API response for app id '{appId}'", appId);
                return null;
            }
        }

        public async Task<NumberOfCurrentPlayersDTO?> StoreNumberOfCurrentPlayersInCache(int appId,
            CancellationToken cancellationToken = default)
        {
            var url = GetFormattedPlayerCountUrl(appId);

            try
            {
                var dto = await GetDtoAsync<NumberOfCurrentPlayersDTO>(url, cancellationToken);

                if (dto is null)
                    return null;

                await _cacheService.SetDtoAsync<NumberOfCurrentPlayersDTO>(GetCacheKey(appId), dto, cancellationToken);

                return dto;
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Error storing number of current players for app id '{appId}'", appId);
                return null;
            }
        }

        public string GetCacheKey(int appId) => $"NumberOfCurrentPlayers_{appId}";

        private string GetFormattedPlayerCountUrl(int appId)
        {
            return UrlFormatter.GetFormattedUrl(new GetNumberOfCurrentPlayersUrl(appId));
        }
    }
}
