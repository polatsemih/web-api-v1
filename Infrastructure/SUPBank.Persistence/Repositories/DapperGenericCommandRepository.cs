using Dapper;
using System.Data;
using SUPBank.Application.Interfaces.Context;
using SUPBank.Application.Interfaces.Repositories;

namespace SUPBank.Persistence.Repositories
{
    public abstract class DapperGenericCommandRepository<T> : DapperGenericRepository<T>, IGenericCommandRepository<T>
    {
        public DapperGenericCommandRepository(IDapperContext dapperContext, string tableName) : base(dapperContext, tableName) { }

        public async Task<int> CommandAsync(string storedProcedure, object parameters, CancellationToken cancellationToken)
        {
            return await _dapperContext.QueryAsync(async (connection, transaction) =>
            {
                return await connection.QuerySingleAsync<int>(sql: storedProcedure, param: parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
            }, cancellationToken);
        }

        public async Task<long?> CreateAndGetIdAsync(string storedProcedure, object parameters, CancellationToken cancellationToken)
        {
            return await _dapperContext.QueryAsync(async (connection, transaction) =>
            {
                return await connection.QuerySingleOrDefaultAsync<long?>(sql: storedProcedure, param: parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
            }, cancellationToken);
        }
    }
}
