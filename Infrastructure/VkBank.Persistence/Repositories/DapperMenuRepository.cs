using Dapper;
using System.Data;
using VkBank.Application.Interfaces.Context;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Domain.Entities;

namespace VkBank.Persistence.Repositories
{
    public class DapperMenuRepository : DapperGenericRepository<Menu>, IMenuRepository
    {
        public DapperMenuRepository(IDapperContext dapperContext) : base(dapperContext, "Menu")
        {

        }

        public async Task<IEnumerable<Menu>> GetAllMenuAsync(CancellationToken cancellationToken)
        {
            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QueryAsync<Menu>("Menu.GetAllMenus", commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }


        public async Task<Menu?> GetMenuByIdAsync(long id, CancellationToken cancellationToken)
        {
            var parameters = new { Id = id };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleOrDefaultAsync<Menu>("Menu.GetMenuById", parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }


        public bool CreateMenu(Menu menu)
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

        public async Task<bool> CreateMenuAsync(Menu menu, CancellationToken cancellationToken)
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


        public long? CreateMenuAndGetId(Menu menu)
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

        public async Task<long?> CreateMenuAndGetIdAsync(Menu menu, CancellationToken cancellationToken)
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


        public bool UpdateMenu(Menu menu)
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

        public async Task<bool> UpdateMenuAsync(Menu menu, CancellationToken cancellationToken)
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
    }
}
