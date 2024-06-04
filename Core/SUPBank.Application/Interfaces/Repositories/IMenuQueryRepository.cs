using SUPBank.Domain.Entities;

namespace SUPBank.Application.Interfaces.Repositories
{
    public interface IMenuQueryRepository : IGenericQueryRepository<EntityMenu>
    {
        public Task<bool> IsIdExistsInMenuAsync(long id, CancellationToken cancellationToken);
        public Task<bool> IsScreenCodeExistsInMenuAsync(int screenCode, CancellationToken cancellationToken);
        public Task<bool> IsParentIdExistsInMenuAsync(long parentId, CancellationToken cancellationToken);
        public Task<bool> IsMenuIdExistsInMenuHAsync(long menuId, CancellationToken cancellationToken);
        public Task<bool> IsScreenCodeExistsInMenuHAsync(int screenCode, CancellationToken cancellationToken);
        public Task<bool> IsRollbackTokenExistsInMenuHAsync(Guid rollbackToken, CancellationToken cancellationToken);
        public Task<EntityMenu?> GetMenuScreenCodeByIdAsync(long id, CancellationToken cancellationToken);

        public Task<IEnumerable<EntityMenu>> GetAllMenusAsync(CancellationToken cancellationToken);
        public Task<EntityMenu?> GetMenuByIdAsync(long id, CancellationToken cancellationToken);
        public Task<EntityMenu?> GetMenuByIdInMenuHAsync(long id, CancellationToken cancellationToken);
        public Task<IEnumerable<EntityMenu>> GetMenuByIdWithSubMenusAsync(long id, CancellationToken cancellationToken);
        public Task<IEnumerable<EntityMenu>> SearchMenusAsync(string keyword, CancellationToken cancellationToken);
    }
}
