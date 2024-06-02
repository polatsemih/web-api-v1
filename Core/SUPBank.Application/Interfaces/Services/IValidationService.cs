namespace SUPBank.Application.Interfaces.Services
{
    /// <summary>
    /// Defines a service for performing validation asynchronously.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Validates a request asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request to validate.</typeparam>
        /// <param name="request">The request to validate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task representing the asynchronous operation. 
        /// The task result contains a string representing the validation result, 
        /// or null if the validation succeeds without any errors.
        /// </returns>
        Task<string?> ValidateAsync<TRequest>(TRequest request, CancellationToken cancellationToken);
    }
}
