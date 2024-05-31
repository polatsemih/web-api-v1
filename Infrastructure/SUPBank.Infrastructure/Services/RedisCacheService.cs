using StackExchange.Redis;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Domain.Contstants;

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
                _logger.LogInformation(string.Format(Cache.CacheMiss, key));
                return default;
            }

            _logger.LogInformation(string.Format(Cache.CacheHit, key));

            var cachedDataString = cachedData.ToString();
            if (string.IsNullOrEmpty(cachedDataString))
            {
                _logger.LogInformation(string.Format(Cache.CacheNullOrEmpty, key));
                return default;
            }

            return _serializer.Deserialize<T>(cachedDataString);
        }

        public async Task<T?> GetCacheAsync<T>(string key)
        {
            var cachedData = await _database.StringGetAsync(key);
            if (cachedData.IsNullOrEmpty)
            {
                _logger.LogInformation(string.Format(Cache.CacheMiss, key));
                return default;
            }

            _logger.LogInformation(string.Format(Cache.CacheHit, key));

            var cachedDataString = cachedData.ToString();
            if (string.IsNullOrEmpty(cachedDataString))
            {
                _logger.LogInformation(string.Format(Cache.CacheNullOrEmpty, key));
                return default;
            }

            return _serializer.Deserialize<T>(cachedDataString);
        }


        public bool AddCache(string key, object value, TimeSpan? duration = null)
        {
            var serializedValue = _serializer.Serialize(value);
            bool result = _database.StringSet(key: key, value: serializedValue, expiry: duration);
            if (result)
            {
                _logger.LogInformation(string.Format(Cache.CacheSetSuccess, key, duration != null ? duration.ToString() : "Infinite"));
            }
            else
            {
                _logger.LogInformation(string.Format(Cache.CacheSetFail, key));
            }

            return result;
        }

        public async Task<bool> AddCacheAsync(string key, object value, TimeSpan? duration = null)
        {
            var serializedValue = _serializer.Serialize(value);
            bool result = await _database.StringSetAsync(key: key, value: serializedValue, expiry: duration);
            if (result)
            {
                _logger.LogInformation(string.Format(Cache.CacheSetSuccess, key, duration != null ? duration.ToString() : "Infinite"));
            }
            else
            {
                _logger.LogInformation(string.Format(Cache.CacheSetFail, key));
            }
            return result;
        }


        public bool RemoveCache(string key)
        {
            bool result = _database.KeyDelete(key);
            if (result)
            {
                _logger.LogInformation(string.Format(Cache.CacheRemoveSuccess, key));
            }
            else
            {
                _logger.LogInformation(string.Format(Cache.CacheRemoveFail, key));
            }
            return result;
        }

        public async Task<bool> RemoveCacheAsync(string key)
        {
            bool result = await _database.KeyDeleteAsync(key);
            if (result)
            {
                _logger.LogInformation(string.Format(Cache.CacheRemoveSuccess, key));
            }
            else
            {
                _logger.LogInformation(string.Format(Cache.CacheRemoveFail, key));
            }
            return result;
        }
    }
}
