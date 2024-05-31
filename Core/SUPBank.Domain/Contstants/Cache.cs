namespace SUPBank.Domain.Contstants
{
    public class Cache
    {
        public const string CacheKeyMenu = "CacheAllMenu";

        public const string CacheMiss = "Redis Cache miss for key: {0}";
        public const string CacheHit = "Redis Cache hit for key: {0}";
        public const string CacheNullOrEmpty = "Redis Cache null or empty for key: {0}";
        public const string CacheSetSuccess = "Redis Cache set for key: {0} with duration: {1}";
        public const string CacheSetFail = "Redis Cache failed to set for key: {0}";
        public const string CacheRemoveSuccess = "Redis Cache removed for key: {0}";
        public const string CacheRemoveFail = "Redis Cache could not removed for key: {0}";
    }
}
