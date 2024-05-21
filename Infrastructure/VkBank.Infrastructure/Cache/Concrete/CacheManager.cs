using Microsoft.Extensions.Caching.Memory;
using VkBank.Infrastructure.Cache.Abstract;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace VkBank.Infrastructure.Cache.Concrete
{
    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheManager> _logger;

        public CacheManager(IMemoryCache memoryCache, ILogger<CacheManager> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public T? GetCache<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out var cachedObject))
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}", key);
                return JsonConvert.DeserializeObject<T>((string)cachedObject);
            }
            _logger.LogInformation("Cache miss for key: {CacheKey}", key);
            return default;
        }

        public void AddCache(string key, object value, TimeSpan duration)
        {
            var serializedValue = JsonConvert.SerializeObject(value);

            if (duration == TimeSpan.Zero)
            {
                _memoryCache.Set(key, serializedValue, new MemoryCacheEntryOptions());
                _logger.LogInformation("Cache set for key: {CacheKey} with infinite duration", key);
            }
            else
            {
                _memoryCache.Set(key, serializedValue, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = duration
                });
                _logger.LogInformation("Cache set for key: {CacheKey} with duration: {CacheDuration}", key, duration);
            }
        }

        public void RemoveCache(string key)
        {
            _memoryCache.Remove(key);
            _logger.LogInformation("Cache removed for key: {CacheKey}", key);
        }
    }
}
