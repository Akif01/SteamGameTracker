
namespace SteamGameTracker.Services
{
    public interface ICacheService
    {
        Task<TDto?> GetDtoAsync<TDto>(string cacheKey, CancellationToken cancellationToken = default, bool removeCorruptedEntryOnException = true) where TDto : class;
        Task<string?> GetStringAsync(string cacheKey, CancellationToken cancellationToken = default);
        Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default);
        Task SetDtoAsync<TDto>(string cacheKey, TDto dto, CancellationToken cancellationToken = default) where TDto : class;
    }
}