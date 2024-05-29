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

        public T? GetCache<T>(string key, string source)
        {
            var cachedData = _database.StringGet(key);
            if (cachedData.IsNullOrEmpty)
            {
                _logger.LogInformation($"Redis Cache miss for key: {key} from source: {source}");
                return default;
            }

            _logger.LogInformation($"Redis Cache hit for key: {key} from source: {source}");

            var cachedDataString = cachedData.ToString();
            if (string.IsNullOrEmpty(cachedDataString))
            {
                _logger.LogInformation($"Redis Cache data string null or empty for key: {key} from source: {source}");
                return default;
            }

            return _serializer.Deserialize<T>(cachedDataString);
        }

        public async Task<T?> GetCacheAsync<T>(string key, string source)
        {
            var cachedData = await _database.StringGetAsync(key);
            if (cachedData.IsNullOrEmpty)
            {
                _logger.LogInformation($"Redis Cache miss for key: {key} from source: {source}");
                return default;
            }

            _logger.LogInformation($"Redis Cache hit for key: {key} from source: {source}");

            var cachedDataString = cachedData.ToString();
            if (string.IsNullOrEmpty(cachedDataString))
            {
                _logger.LogInformation($"Redis Cache data string null or empty for key: {key} from source: {source}");
                return default;
            }
           
            return _serializer.Deserialize<T>(cachedDataString);
        }


        public void AddCache(string key, object value, TimeSpan duration, string source)
        {
            var serializedValue = _serializer.Serialize(value);

            if (duration == TimeSpan.Zero)
            {
                _database.StringSet(key, serializedValue);
                _logger.LogInformation($"Redis Cache set for key: {key} with infinite duration from source: {source}");
            }
            else
            {
                _database.StringSet(key, serializedValue, duration);
                _logger.LogInformation($"Redis Cache set for key: {key} with duration: {duration} from source: {source}");
            }
        }

        public async Task AddCacheAsync(string key, object value, TimeSpan duration, string source)
        {
            var serializedValue = _serializer.Serialize(value);

            if (duration == TimeSpan.Zero)
            {
                await _database.StringSetAsync(key, serializedValue);
                _logger.LogInformation($"Redis Cache set for key: {key} with infinite duration from source: {source}");
            }
            else
            {
                await _database.StringSetAsync(key, serializedValue, duration);
                _logger.LogInformation($"Redis Cache set for key: {key} with duration: {duration} from source: {source}");
            }
        }


        public void RemoveCache(string key, string source)
        {
            _database.KeyDelete(key);
            _logger.LogInformation($"Redis Cache removed for key: {key} from source: {source}");
        }

        public async Task RemoveCacheAsync(string key, string source)
        {
            await _database.KeyDeleteAsync(key);
            _logger.LogInformation($"Redis Cache removed for key: {key} from source: {source}");
        }
    }
}
