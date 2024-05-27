using Microsoft.Extensions.Caching.Memory;
using SUPBank.Application.Interfaces.Services;
using System.Reflection;

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
            if (_memoryCache.TryGetValue(key, out string cachedObject) == true && cachedObject != null)
            {
                _logger.LogInformation($"Cache hit for key: {key} from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
                return _serializer.Deserialize<T>(cachedObject);
            }
            _logger.LogInformation($"Cache miss for key: {key} from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
            return default;
        }

        public void AddCache(string key, object value, TimeSpan duration)
        {
            var serializedValue = _serializer.Serialize(value);

            if (duration == TimeSpan.Zero)
            {
                _memoryCache.Set(key, serializedValue, new MemoryCacheEntryOptions());
                _logger.LogInformation($"Cache set for key: {key} with infinite duration from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
            }
            else
            {
                _memoryCache.Set(key, serializedValue, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = duration
                });
                _logger.LogInformation($"Cache set for key: {key} with duration: {duration} from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
            }
        }

        public void RemoveCache(string key)
        {
            _memoryCache.Remove(key);
            _logger.LogInformation($"Cache removed for key: {key} from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
        }
    }
}
