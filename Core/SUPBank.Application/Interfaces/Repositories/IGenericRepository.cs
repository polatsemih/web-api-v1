namespace SUPBank.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T>
    {
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T?> GetByIdAsync(long id);
        public Task CreateAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(long id);
    }
}
