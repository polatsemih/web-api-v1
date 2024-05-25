using Dapper;
using System.Data;
using VkBank.Application.Interfaces.Context;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Domain.Entities.Attributes;

namespace VkBank.Persistence.Repositories
{
    public abstract class DapperGenericRepository<T> : IGenericRepository<T>
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
                .Where(c => c.Name != "Id" && !Attribute.IsDefined(c, typeof(DapperIgnoreAttribute)))
                .Select(c => c.Name);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            string query = $"SELECT * FROM {_tableName}";

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QueryAsync<T>(sql: query);
            });
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            string query = $"SELECT * FROM {_tableName} WHERE Id = @Id";
            var parameters = new { Id = id };

            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QueryFirstOrDefaultAsync<T>(sql: query, param: parameters);
            });
        }

        public async Task CreateAsync(T entity)
        {
            IEnumerable<string> columns = GetColumns();
            string stringOfColumns = string.Join(", ", columns);
            string stringOfParameters = string.Join(", ", columns.Select(c => "@" + c));
            string query = $"INSERT INTO {_tableName} ({stringOfColumns}) VALUES ({stringOfParameters})";

            await _dapperContext.ExecuteAsync(async (connection) =>
            {
                await connection.ExecuteAsync(sql: query, param: entity);
            });
        }

        public async Task UpdateAsync(T entity)
        {
            IEnumerable<string> columns = GetColumns();
            string stringOfColumns = string.Join(", ", columns.Select(c => $"{c} = @{c}"));
            string query = $"UPDATE {_tableName} SET {stringOfColumns} WHERE Id = @Id";

            await _dapperContext.ExecuteAsync(async (connection) =>
            {
                await connection.ExecuteAsync(sql: query, param: entity);
            });
        }

        public async Task DeleteAsync(long id)
        {
            string query = $"DELETE FROM {_tableName} WHERE Id = @Id";
            var parameters = new { Id = id };

            await _dapperContext.ExecuteAsync(async (connection) =>
            {
                await connection.ExecuteAsync(sql: query, param: parameters);
            });
        }
    }
}
