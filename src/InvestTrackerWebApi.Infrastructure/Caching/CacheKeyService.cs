namespace InvestTrackerWebApi.Infrastructure.Caching;

using InvestTrackerWebApi.Application.Caching;

public class CacheKeyService : ICacheKeyService
{
    public string GetCacheKey(string name, object id) => $"{name}-{id}";
}
