using SteamGameTracker.Models;
using SteamGameTracker.Services.API;

namespace SteamGameTracker.Services
{
    internal class SteamDataBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<SteamDataBackgroundService> _logger;
        private readonly SemaphoreSlim _throttler;
        private readonly TimeSpan _apiCallDelay = TimeSpan.FromMilliseconds(250); // 4 requests per second max
        private readonly int _batchSize = 20; // Process in batches of 20
        private readonly int _numberUpdateIntervalHours = 6; // Only update once every 6 hours

        public SteamDataBackgroundService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<SteamDataBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _throttler = new SemaphoreSlim(1, 1); // Ensure only one API call at a time
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var appListService = scope.ServiceProvider.GetRequiredService<IAppListService>();
                    var playerNumberService = scope.ServiceProvider.GetRequiredService<IPlayerNumberService>();
                    var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

                    await UpdateSteamGameData(appListService, playerNumberService, cacheService, stoppingToken);

                    // Run this service once every 6 hours
                    await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Steam data service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task UpdateSteamGameData(IAppListService appListService, 
            IPlayerNumberService playerNumberService, 
            ICacheService cacheService, 
            CancellationToken cancellationToken)
        {
            var appsModel = await appListService.GetApps(cancellationToken);
            if (appsModel?.Apps is null || appsModel.Apps.Count == 0)
            {
                _logger.LogWarning("No apps retrieved from Steam API");
                return;
            }

            var relevantApps = RelevantApps(appsModel);
            _logger.LogInformation("Retrieved '{Count}' apps from Steam API, relevant app count is '{RelevantAppCount}'", 
                appsModel.Apps.Count, 
                relevantApps.Count);

            // Process in batches
            for (int i = 0; i < relevantApps.Count; i += _batchSize)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var batch = relevantApps.Skip(i).Take(_batchSize).ToList();
                await ProcessAppBatch(playerNumberService, cacheService, batch, cancellationToken);

                // Delay between batches to be extra cautious
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

                _logger.LogInformation("Processed {Current}/{Total} apps",
                    Math.Min(i + _batchSize, relevantApps.Count),
                    relevantApps.Count);
            }
        }

        private List<AppModel> RelevantApps(AppsModel appsModel)
        {
            return appsModel.Apps.Where(x => !string.IsNullOrEmpty(x.Name)).ToList();
        }

        private async Task ProcessAppBatch(IPlayerNumberService playerNumberService, 
            ICacheService cacheService, 
            List<AppModel> apps, 
            CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            foreach (var app in apps)
            {
                // Only add to processing if we need to update
                if (await ShouldUpdatePlayerCount(playerNumberService, cacheService, app.Id, cancellationToken))
                {
                    tasks.Add(ProcessSingleApp(playerNumberService, app, cancellationToken));
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task<bool> ShouldUpdatePlayerCount(IPlayerNumberService playerNumberService, 
            ICacheService cacheService, 
            int appId, 
            CancellationToken cancellationToken)
        {
            string cacheKey = playerNumberService.GetCacheKey(appId);
            var lastUpdate = await cacheService.GetLastUpdateTimeAsync(cacheKey, cancellationToken);

            // If we have data that is less than the specified interval hours old, skip update
            if (lastUpdate.HasValue && DateTime.UtcNow - lastUpdate.Value < TimeSpan.FromHours(_numberUpdateIntervalHours))
                return false;

            return true;
        }

        private async Task ProcessSingleApp(IPlayerNumberService playerNumberService, AppModel app, CancellationToken cancellationToken)
        {
            try
            {
                // Ensure we do not exceed rate limits
                await _throttler.WaitAsync(cancellationToken);
                try
                {
                    await playerNumberService.StoreNumberOfCurrentPlayersInCache(app.Id, cancellationToken);
                    await Task.Delay(_apiCallDelay, cancellationToken);
                }
                finally
                {
                    // Always release the semaphore
                    _throttler.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing app {AppId} ({AppName})", app.Id, app.Name);
            }
        }
    }
}
