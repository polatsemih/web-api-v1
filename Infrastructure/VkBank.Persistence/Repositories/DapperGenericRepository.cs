using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VkBank.Application.Interfaces.Context;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Domain.Common;
using VkBank.Persistence.Context;
using static Dapper.SqlMapper;

namespace VkBank.Persistence.Repositories
{
    public abstract class DapperGenericRepository<T> : IGenericRepository<T> where T : EntityBase
    {
        public IDapperContext DapperContext;
        private readonly string _tableName;

        public DapperGenericRepository(IDapperContext dapperContext, string tableName)
        {
            DapperContext = dapperContext;
            _tableName = tableName;
        }

        private IEnumerable<string> GetColumns()
        {
            return typeof(T)
                .GetProperties()
                .Where(e => e.Name != "Id"
                && !e.PropertyType.GetTypeInfo().IsGenericType
                && !Attribute.IsDefined(e, typeof(DapperIgnoreAttribute)))
                .Select(e => e.Name);
        }

        public List<T> GetAll()
        {
            var query = $"SELECT * FROM {_tableName}";

            using (var connection = DapperContext.GetConnection())
            {
                //connection.OpenAsync().Wait();

                connection.Open();

                return (List<T>)connection.Query<T>(query);
            }
        }

        public T GetById(long id)
        {
            var query = $"SELECT * FROM {_tableName} WHERE Id = @Id";

            using (var connection = DapperContext.GetConnection())
            {
                connection.Open();
                return connection.QueryFirst<T>(query);
            }
        }

        public void Add(T entity)
        {
            var columns = GetColumns();
            var stringOfColumns = string.Join(", ", columns);
            var stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));
            var query = $"INSERT INTO {_tableName} ({stringOfColumns}) VALUES ({stringOfParameters})";

            DapperContext.Execute((connection) =>
            {
                connection.Execute(query, entity);
            });
        }

        public void Update(T entity)
        {
            var columns = GetColumns();
            var stringOfColumns = string.Join(", ", columns.Select(e => $"{e} = @{e}"));
            var query = $"update {_tableName} set {stringOfColumns} where Id = @Id";

            DapperContext.Execute((conn) =>
            {
                conn.Execute(query, entity);
            });
        }

        public void Delete(T entity)
        {
            var query = $"DELETE FROM {_tableName} WHERE Id = @Id";

            DapperContext.Execute((connection) =>
            {
                connection.Execute(query, entity);
            });
        }
    }
}
