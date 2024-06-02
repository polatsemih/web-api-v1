using Microsoft.Extensions.DependencyInjection;
using SUPBank.Application.Interfaces.Context;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Persistence.Context;
using SUPBank.Persistence.Repositories;

namespace SUPBank.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceDependencies(this IServiceCollection services)
        {
            // Register dapper
            services.AddScoped<IDapperContext, DapperContext>();

            // Register menu repositories
            services.AddScoped<IMenuQueryRepository, DapperMenuQueryRepository>();
            services.AddScoped<IMenuCommandRepository, DapperMenuCommandRepository>();
        }
    }
}