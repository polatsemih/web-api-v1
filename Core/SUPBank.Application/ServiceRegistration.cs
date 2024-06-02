using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using SUPBank.Application.Features.Menu.Commands.Requests;
using SUPBank.Application.Features.Menu.Queries.Requests;
using SUPBank.Application.Validations.Menu;

namespace SUPBank.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationDependencies(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Register MediatR services
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            // Register AutoMapper
            services.AddAutoMapper(assembly);

            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(assembly);

            // Register menu validators for specific requests
            services.AddTransient<IValidator<SearchMenuQueryRequest>, SearchMenuValidator>();
            services.AddTransient<IValidator<CreateMenuCommandRequest>, CreateMenuValidator>();
            services.AddTransient<IValidator<DeleteMenuCommandRequest>, DeleteMenuValidator>();
            services.AddTransient<IValidator<GetMenuByIdQueryRequest>, GetMenuByIdValidator>();
            services.AddTransient<IValidator<GetMenuByIdWithSubMenusQueryRequest>, GetMenuByIdWithSubMenusValidator>();
            services.AddTransient<IValidator<RollbackMenuByIdCommandRequest>, RollbackMenuByIdValidator>();
            services.AddTransient<IValidator<RollbackMenuByScreenCodeCommandRequest>, RollbackMenuByScreenCodeValidator>();
            services.AddTransient<IValidator<RollbackMenusByTokenCommandRequest>, RollbackMenuByTokenValidator>();
            services.AddTransient<IValidator<SearchMenuQueryRequest>, SearchMenuValidator>();
            services.AddTransient<IValidator<UpdateMenuCommandRequest>, UpdateMenuValidator>();
        }
    }
}
