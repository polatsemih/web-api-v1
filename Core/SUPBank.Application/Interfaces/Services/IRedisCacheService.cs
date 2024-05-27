namespace SUPBank.Application.Interfaces.Services
{
    public interface IRedisCacheService
    {
        public T? GetCache<T>(string key);
        public Task<T?> GetCacheAsync<T>(string key);

        public void AddCache(string key, object value, TimeSpan duration);
        public Task AddCacheAsync(string key, object value, TimeSpan duration);

        public void RemoveCache(string key);
        public Task RemoveCacheAsync(string key);
    }
}
