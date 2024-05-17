using Dapper;
using VkBank.Application.Interfaces.Context;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Domain.Entities.Common;

namespace VkBank.Persistence.Repositories
{
    public abstract class DapperGenericRepository<T> : IGenericRepository<T> where T : EntityBase
    {
        protected readonly IDapperContext _dapperContext;
        private readonly string _tableName;

        public DapperGenericRepository(IDapperContext dapperContext, string tableName)
        {
            _dapperContext = dapperContext;
            _tableName = tableName;
        }

        private IEnumerable<string> GetColumns()
        {
            return typeof(T)
                .GetProperties()
                .Where(e => e.Name != "Id" && !Attribute.IsDefined(e, typeof(DapperIgnoreAttribute)))
                .Select(e => e.Name);
        }


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var query = $"SELECT * FROM {_tableName}";

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QueryAsync<T>(query);
            });
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            var parameters = new { Id = id };
            var query = $"SELECT * FROM {_tableName} WHERE Id = @Id";

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QueryFirstOrDefaultAsync<T>(query, parameters);
            });
        }

        public async Task CreateAsync(T entity)
        {
            var columns = GetColumns();
            var stringOfColumns = string.Join(", ", columns);
            var stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));
            var query = $"INSERT INTO {_tableName} ({stringOfColumns}) VALUES ({stringOfParameters})";

            await _dapperContext.ExecuteAsync(async (connection) =>
            {
                await connection.ExecuteAsync(query, entity);
            });
        }

        public async Task UpdateAsync(T entity)
        {
            var columns = GetColumns();
            var stringOfColumns = string.Join(", ", columns.Select(e => $"{e} = @{e}"));
            var query = $"UPDATE {_tableName} SET {stringOfColumns} WHERE Id = @Id";

            await _dapperContext.ExecuteAsync(async (connection) =>
            {
                await connection.ExecuteAsync(query, entity);
            });
        }

        public async Task DeleteAsync(long id)
        {
            var parameters = new { Id = id };
            var query = $"DELETE FROM {_tableName} WHERE Id = @Id";

            await _dapperContext.ExecuteAsync(async (connection) =>
            {
                await connection.ExecuteAsync(query, parameters);
            });
        }
    }
}
