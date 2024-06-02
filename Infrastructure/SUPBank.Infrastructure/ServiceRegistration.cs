using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Application.Interfaces.Services.Controllers;
using SUPBank.Domain.Contstants;
using SUPBank.Infrastructure.Services;
using SUPBank.Infrastructure.Services.Controllers;

namespace SUPBank.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IValidationService, ValidationService>();

            // Register logging services
            services.AddLogging();
            services.AddSingleton(typeof(ILogService<>), typeof(LogService<>));
            services.AddSingleton<ISerializerService, SerializerService>();

            // Register memory caching and its service
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, CacheService>();

            // Register redis cache service
            string redisConnectionString = configuration.GetConnectionString("RedisConnection") ?? throw new InvalidOperationException(ExceptionMessages.RedisConnectionStringInvalid);
            ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redisConnectionString) ?? throw new InvalidOperationException(ExceptionMessages.RedisConnectionError);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            services.AddSingleton<IRedisCacheService, RedisCacheService>();

            // Register menu services
            services.AddTransient<IMenuService, MenuService>();
        }
    }
}
