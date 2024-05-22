using VkBank.Domain.Entities;

namespace VkBank.Application.Interfaces.Repositories
{
    public interface IMenuRepository : IGenericRepository<EntityMenu>
    {
        public Task<bool> IsMenuIdExistsAsync(long id, CancellationToken cancellationToken);
        public Task<bool> IsMenuParentIdExistsAsync(long parentId, CancellationToken cancellationToken);
        public Task<bool> IsMenuScreenCodeExistsAsync(int screenCode, CancellationToken cancellationToken);

        public Task<IEnumerable<EntityMenu>> GetAllMenuAsync(CancellationToken cancellationToken);
        public Task<EntityMenu?> GetMenuByIdAsync(long id, CancellationToken cancellationToken);
        public Task<IEnumerable<EntityMenu>> SearchMenuAsync(string keyword, CancellationToken cancellationToken);

        public bool CreateMenu(EntityMenu menu);
        public Task<bool> CreateMenuAsync(EntityMenu menu, CancellationToken cancellationToken);

        public long? CreateMenuAndGetId(EntityMenu menu);
        public Task<long?> CreateMenuAndGetIdAsync(EntityMenu menu, CancellationToken cancellationToken);

        public bool UpdateMenu(EntityMenu menu);
        public Task<bool> UpdateMenuAsync(EntityMenu menu, CancellationToken cancellationToken);

        public bool DeleteMenu(long id);
        public Task<bool> DeleteMenuAsync(long id, CancellationToken cancellationToken);

        public Task<bool> RollbackMenuByIdAsync(long id, CancellationToken cancellationToken);
        public Task<bool> RollbackMenuByScreenCodeAsync(int screenCode, CancellationToken cancellationToken);
        public Task<int?> RollbackMenusByTokenAsync(Guid rollbackToken, CancellationToken cancellationToken);
    }
}
