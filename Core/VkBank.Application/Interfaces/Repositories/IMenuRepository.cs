using VkBank.Domain.Entities;

namespace VkBank.Application.Interfaces.Repositories
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        public Task<IEnumerable<Menu>> GetAllMenuAsync(CancellationToken cancellationToken);

        public Task<Menu?> GetMenuByIdAsync(long id, CancellationToken cancellationToken);

        public bool CreateMenu(Menu menu);
        public Task<bool> CreateMenuAsync(Menu menu, CancellationToken cancellationToken);

        public long? CreateMenuAndGetId(Menu menu);
        public Task<long?> CreateMenuAndGetIdAsync(Menu menu, CancellationToken cancellationToken);

        public bool UpdateMenu(Menu menu);
        public Task<bool> UpdateMenuAsync(Menu menu, CancellationToken cancellationToken);

        public bool DeleteMenu(long id);
        public Task<bool> DeleteMenuAsync(long id, CancellationToken cancellationToken);
    }
}
