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
            var cachedDto = await _cacheService.GetDtoAsync<NumberOfCurrentPlayersDTO>(cacheKey, cancellationToken);

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

            var url = GetFormattedPlayerCountUrl(appId);
            var dto = await GetDtoAsync<NumberOfCurrentPlayersDTO>(url, cancellationToken);

            if (dto is null)
                return null;

            try
            {
                var result = new NumberOfCurrentPlayersModel(dto);
                await _cacheService.SetDtoAsync<NumberOfCurrentPlayersDTO>(cacheKey, dto, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Error building player count model from API response for app id '{appId}'", appId);
                return null;
            }
        }

        private string GetCacheKey(int appId) => $"NumberOfCurrentPlayers_{appId}";

        private string GetFormattedPlayerCountUrl(int appId)
        {
            return UrlFormatter.GetFormattedUrl(new GetNumberOfCurrentPlayersUrl(appId));
        }
    }
}
