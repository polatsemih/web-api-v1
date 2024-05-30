using StackExchange.Redis;
using SUPBank.Application.Interfaces.Services;

namespace SUPBank.Infrastructure.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _database;
        private readonly ILogService<RedisCacheService> _logger;
        private readonly ISerializerService _serializer;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer, ILogService<RedisCacheService> logger, ISerializerService serializer)
        {
            _database = connectionMultiplexer.GetDatabase();
            _logger = logger;
            _serializer = serializer;
        }

        public T? GetCache<T>(string key)
        {
            var cachedData = _database.StringGet(key);
            if (cachedData.IsNullOrEmpty)
            {
                _logger.LogInformation($"Redis Cache miss for key: {key}");
                return default;
            }

            _logger.LogInformation($"Redis Cache hit for key: {key}");

            var cachedDataString = cachedData.ToString();
            if (string.IsNullOrEmpty(cachedDataString))
            {
                _logger.LogInformation($"Redis Cache data string null or empty for key: {key}");
                return default;
            }

            return _serializer.Deserialize<T>(cachedDataString);
        }

        public async Task<T?> GetCacheAsync<T>(string key)
        {
            var cachedData = await _database.StringGetAsync(key);
            if (cachedData.IsNullOrEmpty)
            {
                _logger.LogInformation($"Redis Cache miss for key: {key}");
                return default;
            }

            _logger.LogInformation($"Redis Cache hit for key: {key}");

            var cachedDataString = cachedData.ToString();
            if (string.IsNullOrEmpty(cachedDataString))
            {
                _logger.LogInformation($"Redis Cache data string null or empty for key: {key}");
                return default;
            }

            return _serializer.Deserialize<T>(cachedDataString);
        }


        public bool AddCache(string key, object value, TimeSpan duration)
        {
            var serializedValue = _serializer.Serialize(value);

            if (duration == TimeSpan.Zero)
            {
                _logger.LogInformation($"Redis Cache set for key: {key} with infinite duration");
                return _database.StringSet(key, serializedValue);
            }

            _logger.LogInformation($"Redis Cache set for key: {key} with duration: {duration}");
            return _database.StringSet(key, serializedValue, duration);
        }

        public async Task<bool> AddCacheAsync(string key, object value, TimeSpan duration)
        {
            var serializedValue = _serializer.Serialize(value);

            if (duration == TimeSpan.Zero)
            {
                _logger.LogInformation($"Redis Cache set for key: {key} with infinite duration");
                return await _database.StringSetAsync(key, serializedValue);
            }

            _logger.LogInformation($"Redis Cache set for key: {key} with duration: {duration}");
            return await _database.StringSetAsync(key, serializedValue, duration);
        }


        public bool RemoveCache(string key)
        {
            _logger.LogInformation($"Redis Cache removed for key: {key}");
            return _database.KeyDelete(key);
        }

        public async Task<bool> RemoveCacheAsync(string key)
        {
            _logger.LogInformation($"Redis Cache removed for key: {key}");
            return await _database.KeyDeleteAsync(key);
        }
    }
}
