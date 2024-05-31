using Microsoft.Extensions.Caching.Memory;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Domain.Contstants;

namespace SUPBank.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogService<CacheService> _logger;
        private readonly ISerializerService _serializer;

        public CacheService(IMemoryCache memoryCache, ILogService<CacheService> logger, ISerializerService serializer)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _serializer = serializer;
        }

        public T? GetCache<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out string? cachedObject) && cachedObject != null)
            {
                _logger.LogInformation(string.Format(Cache.CacheHit, key));
                return _serializer.Deserialize<T>(cachedObject);
            }
            _logger.LogInformation(string.Format(Cache.CacheMiss, key));
            return default;
        }

        public void AddCache(string key, object value, TimeSpan? duration = null)
        {
            var serializedValue = _serializer.Serialize(value);

            if (duration == TimeSpan.Zero)
            {
                _memoryCache.Set(key, serializedValue, new MemoryCacheEntryOptions());
            }
            else
            {
                _memoryCache.Set(key, serializedValue, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = duration
                });
            }
            _logger.LogInformation(string.Format(Cache.CacheSetSuccess, key, duration != null ? duration.ToString() : "Infinite"));
        }

        public void RemoveCache(string key)
        {
            _memoryCache.Remove(key);
            _logger.LogInformation(string.Format(Cache.CacheRemoveSuccess, key));
        }
    }
}
