using Microsoft.Extensions.Caching.Distributed;

namespace SteamGameTracker.Services
{
    public class CacheService : ICacheService
    {
        private IDistributedCache _distributedCache;
        private ILogger<CacheService> _logger;

        public CacheService(IDistributedCache distributedCache, ILogger<CacheService> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }
    }
}
