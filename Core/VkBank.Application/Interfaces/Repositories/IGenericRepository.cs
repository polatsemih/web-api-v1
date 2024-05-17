using VkBank.Domain.Entities.Common;

namespace VkBank.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : EntityBase
    {
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T?> GetByIdAsync(long id);
        public Task CreateAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(long id);
    }
}
