namespace VkBank.Infrastructure.Cache.Abstract
{
    public interface ICacheManager
    {
        public T? GetCache<T>(string key);
        public void AddCache(string key, object value, TimeSpan duration);
        public void RemoveCache(string key);
    }
}
