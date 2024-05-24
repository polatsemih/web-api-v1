using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using VkBank.Domain.Contstants;
using VkBank.Infrastructure.Services.Caching.Abstract;
using VkBank.Infrastructure.Services.Caching.Concrete;
using VkBank.Infrastructure.Services.Logging.Abstract;
using VkBank.Infrastructure.Services.Logging.Concrete;
using VkBank.Infrastructure.Services.Serialization.Abstract;
using VkBank.Infrastructure.Services.Serialization.Concrete;

namespace VkBank.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging();
            services.AddSingleton(typeof(ILogService<>), typeof(LogService<>));
            services.AddSingleton<ISerializerService, SerializerService>();

            services.AddMemoryCache();
            services.AddSingleton<ICacheService, CacheService>();

            string redisConnectionString = configuration.GetConnectionString("RedisConnection") ?? throw new InvalidOperationException(ExceptionMessages.RedisConnectionStringInvalid);
            ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redisConnectionString) ?? throw new InvalidOperationException(ExceptionMessages.RedisConnectionError);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        }
    }
}
