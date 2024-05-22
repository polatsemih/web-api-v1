using Microsoft.Extensions.DependencyInjection;
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
        public static void AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddLogging();
            services.AddSingleton(typeof(ILogService<>), typeof(LogService<>));
            services.AddSingleton<ISerializerService, SerializerService>();
        }
    }
}
