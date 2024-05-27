using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SUPBank.Application.Interfaces.Context;
using SUPBank.Application.Interfaces.Services;
using SUPBank.Domain.Contstants;

namespace SUPBank.Persistence.Context
{
    public class DapperContext : IDapperContext
    {
        private readonly string _connectionString;
        private readonly ILogService<DapperContext> _logger;

        public DapperContext(IConfiguration configuration, ILogService<DapperContext> logger)
        {
            _connectionString = configuration.GetConnectionString("SqlConnection") ?? throw new InvalidOperationException();
            _logger = logger;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }


        public void Execute(Action<SqlConnection> action)
        {
            try
            {
                using SqlConnection connection = GetConnection();
                connection.Open();
                action(connection);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ExceptionMessages.SqlException, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ExceptionMessages.Exception, ex);
                throw;
            }
        }

        public async Task ExecuteAsync(Func<SqlConnection, Task> action)
        {
            try
            {
                await using SqlConnection connection = GetConnection();
                await connection.OpenAsync();

                await action(connection);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ExceptionMessages.SqlException, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ExceptionMessages.Exception, ex);
                throw;
            }
        }

        public async Task ExecuteAsync(Func<SqlConnection, Task> action, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await using SqlConnection connection = GetConnection();
                await connection.OpenAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();
                await action(connection);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation($"{ExceptionMessages.OperationCanceledException}: {ex.Message}");
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ExceptionMessages.SqlException, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ExceptionMessages.Exception, ex);
                throw;
            }
        }


        public T Query<T>(Func<SqlConnection, T> query)
        {
            try
            {
                using SqlConnection connection = GetConnection();
                connection.Open();

                return query(connection);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ExceptionMessages.SqlException, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ExceptionMessages.Exception, ex);
                throw;
            }
        }

        public async Task<T> QueryAsync<T>(Func<SqlConnection, Task<T>> query)
        {
            try
            {
                await using SqlConnection connection = GetConnection();
                await connection.OpenAsync();

                return await query(connection);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ExceptionMessages.SqlException, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ExceptionMessages.Exception, ex);
                throw;
            }
        }

        public async Task<T> QueryAsync<T>(Func<SqlConnection, Task<T>> query, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await using SqlConnection connection = GetConnection();
                await connection.OpenAsync(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();
                return await query(connection);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation($"{ExceptionMessages.OperationCanceledException}: {ex.Message}");
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ExceptionMessages.SqlException, ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ExceptionMessages.Exception, ex);
                throw;
            }
        }
    }
}
