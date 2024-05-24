using Dapper;
using System.Data;
using VkBank.Application.Interfaces.Context;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Domain.Entities;

namespace VkBank.Persistence.Repositories
{
    public class DapperMenuRepository : DapperGenericRepository<EntityMenu>, IMenuRepository
    {
        public DapperMenuRepository(IDapperContext dapperContext) : base(dapperContext, "Menu")
        {

        }

        public async Task<bool> IsIdExistsInMenuAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };

            int isExists = await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.IsIdExistsInMenu", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);

            return isExists == 1;
        }

        public async Task<bool> IsScreenCodeExistsInMenuAsync(int screenCode, CancellationToken cancellationToken)
        {
            var parameters = new { ScreenCode = screenCode };

            int isExists = await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.IsScreenCodeExistsInMenu", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);

            return isExists == 1;
        }

        public async Task<bool> IsParentIdExistsInMenuAsync(long parentId, CancellationToken cancellationToken)
        {
            var parameters = new { ParentId = parentId };

            int isExists = await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.IsParentIdExistsInMenu", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);

            return isExists == 1;
        }

        public async Task<bool> IsMenuIdExistsInMenuHAsync(long menuId, CancellationToken cancellationToken)
        {
            var parameters = new { MenuId = menuId };

            int isExists = await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.IsMenuIdExistsInMenuH", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);

            return isExists == 1;
        }

        public async Task<bool> IsScreenCodeExistsInMenuHAsync(int screenCode, CancellationToken cancellationToken)
        {
            var parameters = new { ScreenCode = screenCode };

            int isExists = await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.IsScreenCodeExistsInMenuH", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);

            return isExists == 1;
        }

        public async Task<bool> IsRollbackTokenExistsInMenuHAsync(Guid rollbackToken, CancellationToken cancellationToken)
        {
            var parameters = new { RollbackToken = rollbackToken };

            int isExists = await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.IsRollbackTokenExistsInMenuH", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);

            return isExists == 1;
        }


        public async Task<IEnumerable<EntityMenu>> GetAllMenusAsync(CancellationToken cancellationToken)
        {
            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QueryAsync<EntityMenu>("Menu.GetAllMenus", commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<EntityMenu?> GetMenuByIdAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleOrDefaultAsync<EntityMenu>("Menu.GetMenuById", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<IEnumerable<EntityMenu>> GetMenuByIdWithSubMenusAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QueryAsync<EntityMenu>("Menu.GetMenuByIdWithSubMenus", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<IEnumerable<EntityMenu>> SearchMenusAsync(string keyword, CancellationToken cancellationToken)
        {
            var parameters = new { Keyword = keyword };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QueryAsync<EntityMenu>("Menu.SearchMenus", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }


        public async Task<bool> CreateMenuAsync(EntityMenu menu, CancellationToken cancellationToken)
        {
            var parameters = new
            {
                menu.ParentId,
                menu.Name_TR,
                menu.Name_EN,
                menu.ScreenCode,
                menu.Type,
                menu.Priority,
                menu.Keyword,
                menu.Icon,
                menu.IsGroup,
                menu.IsNew,
                menu.NewStartDate,
                menu.NewEndDate,
                menu.IsActive
            };

            int rowsAffected = await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.CreateMenu", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);

            return rowsAffected > 0;
        }

        public async Task<long?> CreateMenuAndGetIdAsync(EntityMenu menu, CancellationToken cancellationToken)
        {
            var parameters = new
            {
                menu.ParentId,
                menu.Name_TR,
                menu.Name_EN,
                menu.ScreenCode,
                menu.Type,
                menu.Priority,
                menu.Keyword,
                menu.Icon,
                menu.IsGroup,
                menu.IsNew,
                menu.NewStartDate,
                menu.NewEndDate,
                menu.IsActive
            };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleOrDefaultAsync<long?>("Menu.CreateMenuAndGetId", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
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
                menu.Type,
                menu.Priority,
                menu.Keyword,
                menu.Icon,
                menu.IsGroup,
                menu.IsNew,
                menu.NewStartDate,
                menu.NewEndDate,
                menu.IsActive
            };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.UpdateMenu", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<bool> DeleteMenuAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };

            int rowsAffected = await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.DeleteMenu", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);

            return rowsAffected > 0;
        }


        public async Task<int> RollbackMenuByIdAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.RollbackMenuById", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<int> RollbackMenuByScreenCodeAsync(int screenCode, CancellationToken cancellationToken)
        {
            var parameters = new { ScreenCodeInput = screenCode };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.RollbackMenuByScreenCode", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<int> RollbackMenusByTokenAsync(Guid rollbackToken, CancellationToken cancellationToken)
        {
            var parameters = new { RolebackToken = rollbackToken };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.RollbackMenusByToken", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }
    }
}
