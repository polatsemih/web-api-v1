using Microsoft.Extensions.DependencyInjection;
using VkBank.Application.Interfaces.Context;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Persistence.Context;
using VkBank.Persistence.Repositories;

namespace VkBank.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceDependencies(this IServiceCollection services)
        {
            services.AddScoped<IMenuRepository, DapperMenuRepository>();
            services.AddScoped<IDapperContext, DapperContext>();
        }
    }
}
