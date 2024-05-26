using Dapper;
using System.Data;
using SUPBank.Application.Interfaces.Context;
using SUPBank.Application.Interfaces.Repositories;

namespace SUPBank.Persistence.Repositories
{
    public abstract class DapperGenericQueryRepository<T> : DapperGenericRepository<T>, IGenericQueryRepository<T>
    {
        public DapperGenericQueryRepository(IDapperContext dapperContext, string tableName) : base(dapperContext, tableName) { }
        
        public async Task<bool> IsExistsAsync(string storedProcedure, object param, CancellationToken cancellationToken)
        {
            int isExists = await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>(sql: storedProcedure, param: param, commandType: CommandType.StoredProcedure);
            }, cancellationToken);

            return isExists == 1;
        }


        public async Task<IEnumerable<T>> GetAllAsync(string storedProcedure, CancellationToken cancellationToken)
        {
            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QueryAsync<T>(sql: storedProcedure, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(string storedProcedure, object param, CancellationToken cancellationToken)
        {
            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QueryAsync<T>(sql: storedProcedure, param: param, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }


        public async Task<T> GetAsync(string storedProcedure, CancellationToken cancellationToken)
        {
            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<T>(sql: storedProcedure, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<T> GetAsync(string storedProcedure, object param, CancellationToken cancellationToken)
        {
            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<T>(sql: storedProcedure, param: param, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<T?> GetOrDefaultAsync(string storedProcedure, CancellationToken cancellationToken)
        {
            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleOrDefaultAsync<T>(sql: storedProcedure, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<T?> GetOrDefaultAsync(string storedProcedure, object param, CancellationToken cancellationToken)
        {
            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleOrDefaultAsync<T>(sql: storedProcedure, param: param, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }
    }
}
