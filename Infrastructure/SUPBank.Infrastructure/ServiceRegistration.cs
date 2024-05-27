using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Domain.Contstants;
using SUPBank.Infrastructure.Services;

namespace SUPBank.Infrastructure
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

            services.AddSingleton<IRedisCacheService, RedisCacheService>();

            string redisConnectionString = configuration.GetConnectionString("RedisConnection") ?? throw new InvalidOperationException(ExceptionMessages.RedisConnectionStringInvalid);
            ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redisConnectionString) ?? throw new InvalidOperationException(ExceptionMessages.RedisConnectionError);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

        }
    }
}
