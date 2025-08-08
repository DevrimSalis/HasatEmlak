using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Text.Json;

namespace HasatEmlak.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;
        private readonly ConcurrentDictionary<string, bool> _cacheKeys;
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _cacheKeys = new ConcurrentDictionary<string, bool>();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                if (_memoryCache.TryGetValue(key, out var value))
                {
                    _logger.LogDebug($"Cache hit for key: {key}");
                    return (T)value;
                }

                _logger.LogDebug($"Cache miss for key: {key}");
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cache key: {key}");
                return default(T);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration,
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };

                options.RegisterPostEvictionCallback((k, v, reason, state) =>
                {
                    _cacheKeys.TryRemove(k.ToString(), out _);
                    _logger.LogDebug($"Cache key evicted: {k}, Reason: {reason}");
                });

                _memoryCache.Set(key, value, options);
                _cacheKeys.TryAdd(key, true);

                _logger.LogDebug($"Cache set for key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting cache key: {key}");
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                _cacheKeys.TryRemove(key, out _);
                _logger.LogDebug($"Cache removed for key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cache key: {key}");
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var keysToRemove = _cacheKeys.Keys.Where(k => k.Contains(pattern)).ToList();

                foreach (var key in keysToRemove)
                {
                    await RemoveAsync(key);
                }

                _logger.LogDebug($"Cache pattern removed: {pattern}, Keys: {keysToRemove.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cache pattern: {pattern}");
            }
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            var cachedValue = await GetAsync<T>(key);

            if (cachedValue != null && !cachedValue.Equals(default(T)))
            {
                return cachedValue;
            }

            var value = await factory();

            if (value != null && !value.Equals(default(T)))
            {
                await SetAsync(key, value, expiration);
            }

            return value;
        }
    }
}