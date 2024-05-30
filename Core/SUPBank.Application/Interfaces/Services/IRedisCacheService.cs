namespace SUPBank.Application.Interfaces.Services
{
    public interface IRedisCacheService
    {
        public T? GetCache<T>(string key);
        public Task<T?> GetCacheAsync<T>(string key);

        public bool AddCache(string key, object value, TimeSpan duration);
        public Task<bool> AddCacheAsync(string key, object value, TimeSpan duration);

        public bool RemoveCache(string key);
        public Task<bool> RemoveCacheAsync(string key);
    }
}
