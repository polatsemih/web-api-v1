using SUPBank.Application.Interfaces.Context;
using SUPBank.Application.Interfaces.Repositories;
using SUPBank.Domain.Entities;

namespace SUPBank.Persistence.Repositories
{
    public class DapperMenuCommandRepository : DapperGenericCommandRepository<EntityMenu>, IMenuCommandRepository
    {
        public DapperMenuCommandRepository(IDapperContext dapperContext) : base(dapperContext, "Menu") { }

        public async Task<bool> CreateMenuAsync(EntityMenu menu, CancellationToken cancellationToken)
        {
            var parameters = new
            {
                menu.ParentId,
                menu.Name_TR,
                menu.Name_EN,
                menu.ScreenCode,
                menu.WebURL,
                menu.Type,
                menu.Priority,
                menu.IsSearch,
                menu.Keyword,
                menu.Authority,
                menu.Icon,
                menu.IsGroup,
                menu.IsNew,
                menu.NewStartDate,
                menu.NewEndDate,
                menu.IsActive
            };
            int result = await CommandAsync("Menu.CreateMenu", parameters, cancellationToken);
            return result > 0;
        }

        public async Task<long?> CreateMenuAndGetIdAsync(EntityMenu menu, CancellationToken cancellationToken)
        {
            var parameters = new
            {
                menu.ParentId,
                menu.Name_TR,
                menu.Name_EN,
                menu.ScreenCode,
                menu.WebURL,
                menu.Type,
                menu.Priority,
                menu.IsSearch,
                menu.Keyword,
                menu.Authority,
                menu.Icon,
                menu.IsGroup,
                menu.IsNew,
                menu.NewStartDate,
                menu.NewEndDate,
                menu.IsActive
            };
            return await CreateAndGetIdAsync("Menu.CreateMenuAndGetId", parameters, cancellationToken);
        }

        public async Task<int> UpdateMenuAsync(EntityMenu menu, CancellationToken cancellationToken)
        {
            var parameters = new
            {
                menu.Id,
                menu.ParentId,
                menu.Name_TR,
                menu.Name_EN,
                menu.ScreenCode,
                menu.WebURL,
                menu.Type,
                menu.Priority,
                menu.IsSearch,
                menu.Keyword,
                menu.Authority,
                menu.Icon,
                menu.IsGroup,
                menu.IsNew,
                menu.NewStartDate,
                menu.NewEndDate,
                menu.IsActive
            };
            return await CommandAsync("Menu.UpdateMenu", parameters, cancellationToken);
        }

        public async Task<bool> DeleteMenuAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };
            int result = await CommandAsync("Menu.DeleteMenu", parameters, cancellationToken);
            return result > 0;
        }


        public async Task<int> RollbackMenuByIdAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };
            return await CommandAsync("Menu.RollbackMenuById", parameters, cancellationToken);
        }

        public async Task<int> RollbackMenuByScreenCodeAsync(int screenCode, CancellationToken cancellationToken)
        {
            var parameters = new { ScreenCodeInput = screenCode };
            return await CommandAsync("Menu.RollbackMenuByScreenCode", parameters, cancellationToken);
        }

        public async Task<int> RollbackMenusByTokenAsync(Guid rollbackToken, CancellationToken cancellationToken)
        {
            var parameters = new { RolebackToken = rollbackToken };
            return await CommandAsync("Menu.RollbackMenusByToken", parameters, cancellationToken);
        }
    }
}
