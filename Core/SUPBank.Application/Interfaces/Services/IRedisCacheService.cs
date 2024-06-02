namespace SUPBank.Application.Interfaces.Services
{
    /// <summary>
    /// Provides methods for interacting with a Redis cache.
    /// </summary>
    public interface IRedisCacheService
    {
        /// <summary>
        /// Retrieves an item from the cache identified by the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the item to retrieve.</typeparam>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <returns>The item retrieved from the cache, or null if the item is not found.</returns>
        public T? GetCache<T>(string key);

        /// <summary>
        /// Asynchronously retrieves an item from the cache identified by the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the item to retrieve.</typeparam>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <returns>The task representing the asynchronous operation, containing the item retrieved from the cache, or null if the item is not found.</returns>
        public Task<T?> GetCacheAsync<T>(string key);

        /// <summary>
        /// Adds an item to the cache with the specified key and value, optionally setting its expiration duration.
        /// </summary>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <param name="value">The value of the item to be cached.</param>
        /// <param name="duration">The optional duration for which the item should be cached.</param>
        /// <returns>True if the item was successfully added to the cache; otherwise, false.</returns>
        public bool AddCache(string key, object value, TimeSpan? duration = null);

        /// <summary>
        /// Asynchronously adds an item to the cache with the specified key and value, optionally setting its expiration duration.
        /// </summary>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <param name="value">The value of the item to be cached.</param>
        /// <param name="duration">The optional duration for which the item should be cached.</param>
        /// <returns>The task representing the asynchronous operation, containing a Boolean value indicating whether the item was successfully added to the cache.</returns>
        public Task<bool> AddCacheAsync(string key, object value, TimeSpan? duration = null);

        /// <summary>
        /// Removes the item with the specified key from the cache.
        /// </summary>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <returns>True if the item was successfully removed from the cache; otherwise, false.</returns>
        public bool RemoveCache(string key);

        /// <summary>
        /// Asynchronously removes the item with the specified key from the cache.
        /// </summary>
        /// <param name="key">The key used to identify the item in the cache.</param>
        /// <returns>The task representing the asynchronous operation, containing a Boolean value indicating whether the item was successfully removed from the cache.</returns>
        public Task<bool> RemoveCacheAsync(string key);
    }
}
