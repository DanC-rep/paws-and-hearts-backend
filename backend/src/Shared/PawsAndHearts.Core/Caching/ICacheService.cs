using Microsoft.Extensions.Caching.Distributed;

namespace PawsAndHearts.Core.Caching;

public interface ICacheService
{
    Task<T?> GetOrCreateAsync<T>(
        string key, 
        DistributedCacheEntryOptions options, 
        Func<Task<T?>> factory, 
        CancellationToken cancellationToken = default)
        where T : class;
    
    Task<T?> GetAsync<T>(
        string key, 
        CancellationToken cancellationToken = default)
        where T : class;

    Task SetAsync<T>(
        string key, 
        T value, 
        DistributedCacheEntryOptions options, 
        CancellationToken cancellationToken = default)
        where T : class;

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}