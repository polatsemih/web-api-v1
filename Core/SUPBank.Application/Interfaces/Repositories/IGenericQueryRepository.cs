namespace SUPBank.Application.Interfaces.Repositories
{
    public interface IGenericQueryRepository<T> : IGenericRepository<T>
    {
        public Task<bool> IsExistsAsync(string storedProcedure, object param, CancellationToken cancellationToken);

        public Task<IEnumerable<T>> GetAllAsync(string storedProcedure, CancellationToken cancellationToken);
        public Task<IEnumerable<T>> GetAllAsync(string storedProcedure, object param, CancellationToken cancellationToken);

        public Task<T> GetAsync(string storedProcedure, CancellationToken cancellationToken);
        public Task<T> GetAsync(string storedProcedure, object param, CancellationToken cancellationToken);
        public Task<T?> GetOrDefaultAsync(string storedProcedure, CancellationToken cancellationToken);
        public Task<T?> GetOrDefaultAsync(string storedProcedure, object param, CancellationToken cancellationToken);
    }
}
