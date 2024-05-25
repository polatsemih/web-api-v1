using Dapper;
using System.Data;
using VkBank.Application.Interfaces.Context;
using VkBank.Application.Interfaces.Repositories;

namespace VkBank.Persistence.Repositories
{
    public abstract class DapperGenericCommandRepository<T> : DapperGenericRepository<T>, IGenericCommandRepository<T>
    {
        public DapperGenericCommandRepository(IDapperContext dapperContext, string tableName) : base(dapperContext, tableName) { }

        public async Task<int> CommandAsync(string storedProcedure, object parameters, CancellationToken cancellationToken)
        {
            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleAsync<int>(sql: storedProcedure, param: parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }

        public async Task<long?> CreateAndGetIdAsync(string storedProcedure, object parameters, CancellationToken cancellationToken)
        {
            return await _dapperContext.QueryAsync(async (connection) =>
            {
                return await connection.QuerySingleOrDefaultAsync<long?>(sql: storedProcedure, param: parameters, commandType: CommandType.StoredProcedure);
            }, cancellationToken);
        }
    }
}
