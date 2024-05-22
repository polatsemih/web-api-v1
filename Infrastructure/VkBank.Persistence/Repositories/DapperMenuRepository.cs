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
        public async Task<bool> IsMenuIdExistsAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<bool>("Menu.IsMenuIdExists", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<bool> IsMenuParentIdExistsAsync(long parentId, CancellationToken cancellationToken)
        {
            var parameters = new { ParentId = parentId };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<bool>("Menu.IsMenuParentIdExists", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<bool> IsMenuScreenCodeExistsAsync(int screenCode, CancellationToken cancellationToken)
        {
            var parameters = new { ScreenCode = screenCode };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<bool>("Menu.IsMenuScreenCodeExists", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }


        public async Task<IEnumerable<EntityMenu>> GetAllMenuAsync(CancellationToken cancellationToken)
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

        public async Task<IEnumerable<EntityMenu>> SearchMenuAsync(string keyword, CancellationToken cancellationToken)
        {
            var parameters = new { Keyword = keyword };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QueryAsync<EntityMenu>("Menu.GetSearchedMenus", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }


        public bool CreateMenu(EntityMenu menu)
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

            //Directly Executing SP

            //_dapperContext.Execute((connection) =>
            //{
            //    connection.Execute("Menu.CreateMenu", parameters, commandType: CommandType.StoredProcedure);
            //});

            int rowsAffected = _dapperContext.Query<int>((connection) =>
            {
                return connection.QuerySingle<int>("Menu.CreateMenu", parameters, commandType: CommandType.StoredProcedure);
            });

            return rowsAffected > 0;
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


        public long? CreateMenuAndGetId(EntityMenu menu)
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

            return _dapperContext.Query<long?>((connection) =>
            {
                return connection.QuerySingleOrDefault<long?>("Menu.CreateMenuAndGetId", parameters, commandType: CommandType.StoredProcedure);
            });
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


        public bool UpdateMenu(EntityMenu menu)
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

            int rowsAffected = _dapperContext.Query<int>((connection) =>
            {
                return connection.QuerySingle<int>("Menu.UpdateMenu", parameters, commandType: CommandType.StoredProcedure);
            });

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateMenuAsync(EntityMenu menu, CancellationToken cancellationToken)
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

            int rowsAffected = await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.UpdateMenu", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);

            return rowsAffected > 0;
        }


        public bool DeleteMenu(long id)
        {
            var parameters = new { Id = id };

            int rowsAffected = _dapperContext.Query<int>((connection) =>
            {
                return connection.QuerySingle<int>("Menu.DeleteMenu", parameters, commandType: CommandType.StoredProcedure);
            });

            return rowsAffected > 0;
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


        public async Task<bool> RollbackMenuByIdAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };

            int rowsAffected = await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.RollbackMenuById", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);

            return rowsAffected > 0;
        }

        public async Task<bool> RollbackMenuByScreenCodeAsync(int screenCode, CancellationToken cancellationToken)
        {
            var parameters = new { ScreenCodeInput = screenCode };

            int rowsAffected = await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>("Menu.RollbackMenuByScreenCode", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);

            return rowsAffected > 0;
        }

        public async Task<int?> RollbackMenusByTokenAsync(Guid rollbackToken, CancellationToken cancellationToken)
        {
            var parameters = new { RolebackToken = rollbackToken };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleOrDefaultAsync<int?>("Menu.RollbackMenusByToken", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }
    }
}
