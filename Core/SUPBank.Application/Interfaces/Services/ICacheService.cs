namespace SUPBank.Application.Interfaces.Services
{
    public interface ICacheService
    {
        /// <summary>
        /// Retrieves an item from the cache identified by the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the item to retrieve.</typeparam>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <returns>The item retrieved from the cache, or null if the item is not found.</returns>
        public T? GetCache<T>(string key);

        /// <summary>
        /// Adds an item to the cache with the specified key and value, optionally setting its expiration duration.
        /// </summary>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <param name="value">The value of the item to be cached.</param>
        /// <param name="duration">The optional duration for which the item should be cached.</param>
        public void AddCache(string key, object value, TimeSpan? duration = null);

        /// <summary>
        /// Removes the item with the specified key from the cache.
        /// </summary>
        /// <param name="key">The key used to identify the item in the cache.</param>
        public void RemoveCache(string key);
    }
}
