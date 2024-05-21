using Microsoft.Extensions.DependencyInjection;
using VkBank.Infrastructure.Cache.Abstract;
using VkBank.Infrastructure.Cache.Concrete;

namespace VkBank.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddScoped<ICacheManager, CacheManager>();
            services.AddLogging();
        }
    }
}
