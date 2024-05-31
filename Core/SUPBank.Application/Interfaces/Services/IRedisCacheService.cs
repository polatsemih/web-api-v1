namespace SUPBank.Application.Interfaces.Services
{
    public interface IRedisCacheService
    {
        public T? GetCache<T>(string key);
        public Task<T?> GetCacheAsync<T>(string key);

        public bool AddCache(string key, object value, TimeSpan? duration = null);
        public Task<bool> AddCacheAsync(string key, object value, TimeSpan? duration = null);

        public bool RemoveCache(string key);
        public Task<bool> RemoveCacheAsync(string key);
    }
}
