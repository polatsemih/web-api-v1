using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;

namespace VkBank.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationDependencies(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
