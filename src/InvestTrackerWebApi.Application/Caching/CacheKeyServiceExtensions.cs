namespace InvestTrackerWebApi.Application.Caching;

public static class CacheKeyServiceExtensions
{
    public static string GetCacheKey<TEntity>(
        this ICacheKeyService cacheKeyService,
        object id) =>
        cacheKeyService.GetCacheKey(typeof(TEntity).Name, id);
}
