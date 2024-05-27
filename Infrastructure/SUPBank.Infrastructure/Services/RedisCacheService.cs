using StackExchange.Redis;
using SUPBank.Application.Interfaces.Services;
using System.Reflection;

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
                _logger.LogInformation($"Redis Cache miss for key: {key} from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
                return default;
            }

            _logger.LogInformation($"Redis Cache hit for key: {key} from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
            return _serializer.Deserialize<T>(cachedData);
        }

        public async Task<T?> GetCacheAsync<T>(string key)
        {
            var cachedData = await _database.StringGetAsync(key);
            if (cachedData.IsNullOrEmpty)
            {
                _logger.LogInformation($"Redis Cache miss for key: {key} from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
                return default;
            }

            _logger.LogInformation($"Redis Cache hit for key: {key} from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
            return _serializer.Deserialize<T>(cachedData);
        }


        public void AddCache(string key, object value, TimeSpan duration)
        {
            var serializedValue = _serializer.Serialize(value);

            if (duration == TimeSpan.Zero)
            {
                _database.StringSet(key, serializedValue);
                _logger.LogInformation($"Redis Cache set for key: {key} with infinite duration from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
            }
            else
            {
                _database.StringSet(key, serializedValue, duration);
                _logger.LogInformation($"Redis Cache set for key: {key} with duration: {duration} from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
            }
        }

        public async Task AddCacheAsync(string key, object value, TimeSpan duration)
        {
            var serializedValue = _serializer.Serialize(value);

            if (duration == TimeSpan.Zero)
            {
                await _database.StringSetAsync(key, serializedValue);
                _logger.LogInformation($"Redis Cache set for key: {key} with infinite duration from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
            }
            else
            {
                await _database.StringSetAsync(key, serializedValue, duration);
                _logger.LogInformation($"Redis Cache set for key: {key} with duration: {duration} from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
            }
        }


        public void RemoveCache(string key)
        {
            _database.KeyDelete(key);
            _logger.LogInformation($"Redis Cache removed for key: {key} from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
        }

        public async Task RemoveCacheAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
            _logger.LogInformation($"Redis Cache removed for key: {key} from source: {MethodBase.GetCurrentMethod().DeclaringType.FullName}.{MethodBase.GetCurrentMethod().Name}");
        }
    }
}
