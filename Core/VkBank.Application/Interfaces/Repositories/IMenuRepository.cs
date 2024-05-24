using VkBank.Domain.Entities;

namespace VkBank.Application.Interfaces.Repositories
{
    public interface IMenuRepository : IGenericRepository<EntityMenu>
    {
        public Task<bool> IsIdExistsInMenuAsync(long id, CancellationToken cancellationToken);
        public Task<bool> IsScreenCodeExistsInMenuAsync(int screenCode, CancellationToken cancellationToken);
        public Task<bool> IsParentIdExistsInMenuAsync(long parentId, CancellationToken cancellationToken);
        public Task<bool> IsMenuIdExistsInMenuHAsync(long menuId, CancellationToken cancellationToken);
        public Task<bool> IsScreenCodeExistsInMenuHAsync(int screenCode, CancellationToken cancellationToken);
        public Task<bool> IsRollbackTokenExistsInMenuHAsync(Guid rollbackToken, CancellationToken cancellationToken);

        public Task<IEnumerable<EntityMenu>> GetAllMenusAsync(CancellationToken cancellationToken);
        public Task<EntityMenu?> GetMenuByIdAsync(long id, CancellationToken cancellationToken);
        public Task<IEnumerable<EntityMenu>> GetMenuByIdWithSubMenusAsync(long id, CancellationToken cancellationToken);
        public Task<IEnumerable<EntityMenu>> SearchMenusAsync(string keyword, CancellationToken cancellationToken);

        public Task<bool> CreateMenuAsync(EntityMenu menu, CancellationToken cancellationToken);
        public Task<long?> CreateMenuAndGetIdAsync(EntityMenu menu, CancellationToken cancellationToken);
        public Task<int> UpdateMenuAsync(EntityMenu menu, CancellationToken cancellationToken);
        public Task<bool> DeleteMenuAsync(long id, CancellationToken cancellationToken);

        public Task<int> RollbackMenuByIdAsync(long id, CancellationToken cancellationToken);
        public Task<int> RollbackMenuByScreenCodeAsync(int screenCode, CancellationToken cancellationToken);
        public Task<int> RollbackMenusByTokenAsync(Guid rollbackToken, CancellationToken cancellationToken);
    }
}
