using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Application.Interfaces.Context;
using VkBank.Application.Interfaces.Repositories;
using VkBank.Domain.Entities;
using VkBank.Persistence.Context;

namespace VkBank.Persistence.Repositories
{
    public class DapperMenuRepository : DapperGenericRepository<Menu>, IMenuRepository
    {
        public DapperMenuRepository(IDapperContext dapperContext) : base(dapperContext, "Menu")
        {
            
        }

        //public List<Menu> GetMenuByParentId(int Id)
        //{
        //    var query = $"SELECT * FROM Menu WHERE ParentId = {Id} ";

        //    using (var connnection = DapperContext.GetConnection())
        //    {
        //        connnection.Open();
        //        return (List<Menu>)connnection.Query<Menu>(query);
        //    }
        //}
    }
}
