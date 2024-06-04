using SUPBank.Application.Interfaces.Context;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Domain.Entities;

namespace SUPBank.Persistence.Repositories
{
    public class DapperMenuQueryRepository : DapperGenericQueryRepository<EntityMenu>, IMenuQueryRepository
    {
        public DapperMenuQueryRepository(IDapperContext dapperContext) : base(dapperContext, "Menu") { }

        public async Task<bool> IsIdExistsInMenuAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };
            return await IsExistsAsync("Menu.IsIdExistsInMenu", parameters, cancellationToken);
        }

        public async Task<bool> IsScreenCodeExistsInMenuAsync(int screenCode, CancellationToken cancellationToken)
        {
            var parameters = new { ScreenCode = screenCode };
            return await IsExistsAsync("Menu.IsScreenCodeExistsInMenu", parameters, cancellationToken);
        }

        public async Task<bool> IsParentIdExistsInMenuAsync(long parentId, CancellationToken cancellationToken)
        {
            var parameters = new { ParentId = parentId };
            return await IsExistsAsync("Menu.IsParentIdExistsInMenu", parameters, cancellationToken);
        }

        public async Task<bool> IsMenuIdExistsInMenuHAsync(long menuId, CancellationToken cancellationToken)
        {
            var parameters = new { MenuId = menuId };
            return await IsExistsAsync("Menu.IsMenuIdExistsInMenuH", parameters, cancellationToken);
        }

        public async Task<bool> IsScreenCodeExistsInMenuHAsync(int screenCode, CancellationToken cancellationToken)
        {
            var parameters = new { ScreenCode = screenCode };
            return await IsExistsAsync("Menu.IsScreenCodeExistsInMenuH", parameters, cancellationToken);
        }

        public async Task<bool> IsRollbackTokenExistsInMenuHAsync(Guid rollbackToken, CancellationToken cancellationToken)
        {
            var parameters = new { RollbackToken = rollbackToken };
            return await IsExistsAsync("Menu.IsRollbackTokenExistsInMenuH", parameters, cancellationToken);
        }

        public async Task<EntityMenu?> GetMenuScreenCodeByIdAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };
            return await GetAsync("Menu.GetMenuScreenCodeById", parameters, cancellationToken);
        }


        public async Task<IEnumerable<EntityMenu>> GetAllMenusAsync(CancellationToken cancellationToken)
        {
            return await GetAllAsync("Menu.GetAllMenus", cancellationToken);
        }

        public async Task<EntityMenu?> GetMenuByIdAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };
            return await GetAsync("Menu.GetMenuById", parameters, cancellationToken);
        }

        public async Task<EntityMenu?> GetMenuByIdInMenuHAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };
            return await GetAsync("Menu.GetMenuByIdInMenuH", parameters, cancellationToken);
        }

        public async Task<IEnumerable<EntityMenu>> GetMenuByIdWithSubMenusAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };
            return await GetAllAsync("Menu.GetMenuByIdWithSubMenus", parameters, cancellationToken);
        }

        public async Task<IEnumerable<EntityMenu>> SearchMenusAsync(string keyword, CancellationToken cancellationToken)
        {
            var parameters = new { Keyword = keyword };
            return await GetAllAsync("Menu.SearchMenus", parameters, cancellationToken);
        }
    }
}
