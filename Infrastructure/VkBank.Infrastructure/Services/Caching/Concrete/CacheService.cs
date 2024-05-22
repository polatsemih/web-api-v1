using Microsoft.Extensions.Caching.Memory;
using VkBank.Infrastructure.Services.Caching.Abstract;
using VkBank.Infrastructure.Services.Logging.Abstract;
using VkBank.Infrastructure.Services.Serialization.Abstract;

namespace VkBank.Infrastructure.Services.Caching.Concrete
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
            if (_memoryCache.TryGetValue(key, out var cachedObject))
            {
                _logger.LogInformation($"Cache hit for key: {key}");
                return _serializer.Deserialize<T>((string)cachedObject);
            }
            _logger.LogInformation($"Cache miss for key: {key}");
            return default;
        }

        public void AddCache(string key, object value, TimeSpan duration)
        {
            var serializedValue = _serializer.Serialize(value);

            if (duration == TimeSpan.Zero)
            {
                _memoryCache.Set(key, serializedValue, new MemoryCacheEntryOptions());
                _logger.LogInformation($"Cache set for key: {key} with infinite duration");
            }
            else
            {
                _memoryCache.Set(key, serializedValue, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = duration
                });
                _logger.LogInformation($"Cache set for key: {key} with duration: {duration}");
            }
        }

        public void RemoveCache(string key)
        {
            _memoryCache.Remove(key);
            _logger.LogInformation($"Cache removed for key: {key}");
        }
    }
}
