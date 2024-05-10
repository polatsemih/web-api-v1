using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VkBank.Application.Interfaces.Context;

namespace VkBank.Persistence.Context
{
    public class DapperContext : IDapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }

        public SqlConnection GetConnection()
        {
            if (_connectionString == null)
            {
                throw new InvalidOperationException("Connection string is not initialized.");
            }

            return new SqlConnection(_connectionString);
        }

        public void Execute(Action<SqlConnection> @event)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                @event(connection);
            }
        }
    }
}
