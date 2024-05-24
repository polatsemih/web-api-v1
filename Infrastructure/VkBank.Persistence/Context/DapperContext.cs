using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using VkBank.Application.Interfaces.Context;
using VkBank.Domain.Contstants;

namespace VkBank.Persistence.Context
{
    public class DapperContext : IDapperContext
    {
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlConnection") ?? throw new InvalidOperationException(ExceptionMessages.SqlConnectionStringInvalid);
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }


        public void Execute(Action<SqlConnection> action)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();
            action(connection);
        }

        public async Task ExecuteAsync(Func<SqlConnection, Task> action)
        {
            await using SqlConnection connection = GetConnection();
            await connection.OpenAsync();
            await action(connection);
        }

        public async Task ExecuteAsync(Func<SqlConnection, Task> action, CancellationToken cancellationToken)
        {
            await using SqlConnection connection = GetConnection();
            await connection.OpenAsync(cancellationToken);
            await action(connection);
        }


        public T Query<T>(Func<SqlConnection, T> query)
        {
            using SqlConnection connection = GetConnection();
            connection.Open();
            return query(connection);
        }

        public async Task<T> QueryAsync<T>(Func<SqlConnection, Task<T>> query)
        {
            await using SqlConnection connection = GetConnection();
            await connection.OpenAsync();
            return await query(connection);
        }

        public async Task<T> QueryAsync<T>(Func<SqlConnection, Task<T>> query, CancellationToken cancellationToken)
        {
            await using SqlConnection connection = GetConnection();
            await connection.OpenAsync(cancellationToken);
            return await query(connection);
        }
    }
}
