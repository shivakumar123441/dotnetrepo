namespace InvestTrackerWebApi.Application.Caching;

public interface ICacheKeyService
{
    public string GetCacheKey(string name, object id);
}
