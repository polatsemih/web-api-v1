using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkBank.Application.Interfaces.Context
{
    public interface IDapperContext
    {
        public SqlConnection GetConnection();
        public void Execute(Action<SqlConnection> @event);
    }
}
