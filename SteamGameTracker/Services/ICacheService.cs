

namespace SteamGameTracker.Services
{
    public interface ICacheService
    {
        Task<TDto?> GetDtoAsync<TDto>(string cacheKey, bool removeCorruptedEntryOnException = true, CancellationToken cancellationToken = default) where TDto : class;
        Task<DateTime?> GetLastUpdateTimeAsync(string cacheKey, CancellationToken cancellationToken);
        Task<string?> GetStringAsync(string cacheKey, CancellationToken cancellationToken = default);
        Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default);
        Task SetDtoAsync<TDto>(string cacheKey, TDto dto, CancellationToken cancellationToken = default) where TDto : class;
    }
}