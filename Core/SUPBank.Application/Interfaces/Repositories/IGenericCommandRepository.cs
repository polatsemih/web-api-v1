namespace SUPBank.Application.Interfaces.Repositories
{
    public interface IGenericCommandRepository<T> : IGenericRepository<T>
    {
        public Task<int> CommandAsync(string storedProcedure, object parameters, CancellationToken cancellationToken);
        public Task<long?> CreateAndGetIdAsync(string storedProcedure, object parameters, CancellationToken cancellationToken);
    }
}
