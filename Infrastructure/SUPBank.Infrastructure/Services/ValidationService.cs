using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SUPBank.Application.Interfaces.Services;

namespace SUPBank.Infrastructure.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string?> ValidateAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            var validator = _serviceProvider.GetService<IValidator<TRequest>>() ?? throw new ArgumentException($"No validator found for {typeof(TRequest).Name}");
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
            }

            return null;
        }
    }
}
