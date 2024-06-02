namespace SUPBank.Application.Interfaces.Services
{
    public interface IValidationService
    {
        Task<string?> ValidateAsync<TRequest>(TRequest request, CancellationToken cancellationToken);
    }
}
