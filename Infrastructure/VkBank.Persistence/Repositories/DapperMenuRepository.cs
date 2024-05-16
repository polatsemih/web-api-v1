using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
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

        public void CreateMenu(Menu menu)
        {
            var parameters = new
            {
                menu.ParentId,
                menu.Name_TR,
                menu.Name_EN,
                menu.ScreenCode,
                menu.Type,
                menu.Priority,
                menu.Keyword,
                menu.Icon,
                menu.IsGroup,
                menu.IsNew,
                menu.NewStartDate,
                menu.NewEndDate,
                menu.IsActive
            };

            DapperContext.Execute((connection) =>
            {
                connection.Execute("CreateMenu", parameters, commandType: CommandType.StoredProcedure);
            });
        }

        public void UpdateMenu(Menu menu)
        {
            var parameters = new
            {
                menu.Id,
                menu.ParentId,
                menu.Name_TR,
                menu.Name_EN,
                menu.ScreenCode,
                menu.Type,
                menu.Priority,
                menu.Keyword,
                menu.Icon,
                menu.IsGroup,
                menu.IsNew,
                menu.NewStartDate,
                menu.NewEndDate,
                menu.IsActive
            };

            DapperContext.Execute((connection) =>
            {
                connection.Execute("UpdateMenu", parameters, commandType: CommandType.StoredProcedure);
            });
        }

        public void DeleteMenu(long id)
        {
            var parameters = new { Id = id };

            DapperContext.Execute((connection) =>
            {
                connection.Execute("DeleteMenu", parameters, commandType: CommandType.StoredProcedure);
            });
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
