namespace InvestTrackerWebApi.Infrastructure.Caching;
using InvestTrackerWebApi.Application.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class LocalCacheService : ICacheService
{
    private readonly ILogger<LocalCacheService> logger;
    private readonly IMemoryCache cache;

    public LocalCacheService(IMemoryCache cache, ILogger<LocalCacheService> logger)
    {
        this.cache = cache;
        this.logger = logger;
    }

    public T? Get<T>(string key) =>
        this.cache.Get<T>(key);

    public Task<T?> GetAsync<T>(string key, CancellationToken token = default) =>
        Task.FromResult(this.Get<T>(key));

    public void Refresh(string key) =>
        this.cache.TryGetValue(key, out _);

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        this.Refresh(key);
        return Task.CompletedTask;
    }

    public void Remove(string key) =>
        this.cache.Remove(key);

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        this.Remove(key);
        return Task.CompletedTask;
    }

    public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
    {
        if (slidingExpiration is null)
        {
            slidingExpiration = TimeSpan.FromMinutes(10);
        }

        this.cache.Set(key, value, new MemoryCacheEntryOptions { SlidingExpiration = slidingExpiration });
        this.logger.LogDebug($"Added to Cache : {key}", key);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken token = default)
    {
        this.Set(key, value, slidingExpiration);
        return Task.CompletedTask;
    }
}
