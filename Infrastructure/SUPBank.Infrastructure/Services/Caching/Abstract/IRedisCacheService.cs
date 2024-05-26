namespace SUPBank.Infrastructure.Services.Caching.Abstract
{
    public interface IRedisCacheService
    {
        public T? GetCache<T>(string key, string source);
        public Task<T?> GetCacheAsync<T>(string key, string source);

        public void AddCache(string key, object value, TimeSpan duration, string source);
        public Task AddCacheAsync(string key, object value, TimeSpan duration, string source);

        public void RemoveCache(string key, string source);
        public Task RemoveCacheAsync(string key, string source);
    }
}
