using Microsoft.Extensions.Caching.Memory;
using SUPBank.Application.Interfaces.Services;

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

        public T? GetCache<T>(string key, string source)
        {
            if (_memoryCache.TryGetValue(key, out string cachedObject) && cachedObject != null)
            {
                _logger.LogInformation($"Cache hit for key: {key} from source: {source}");
                return _serializer.Deserialize<T>(cachedObject);
            }
            _logger.LogInformation($"Cache miss for key: {key} from source: {source}");
            return default;
        }

        public void AddCache(string key, object value, TimeSpan duration, string source)
        {
            var serializedValue = _serializer.Serialize(value);

            if (duration == TimeSpan.Zero)
            {
                _memoryCache.Set(key, serializedValue, new MemoryCacheEntryOptions());
                _logger.LogInformation($"Cache set for key: {key} with infinite duration from source: {source}");
            }
            else
            {
                _memoryCache.Set(key, serializedValue, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = duration
                });
                _logger.LogInformation($"Cache set for key: {key} with duration: {duration} from source: {source}");
            }
        }

        public void RemoveCache(string key, string source)
        {
            _memoryCache.Remove(key);
            _logger.LogInformation($"Cache removed for key: {key} from source: {source}");
        }
    }
}
