namespace SUPBank.Application.Interfaces.Services
{
    public interface ICacheService
    {
        public T? GetCache<T>(string key);
        public void AddCache(string key, object value, TimeSpan duration);
        public void RemoveCache(string key);
    }
}
