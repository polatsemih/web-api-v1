using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Domain.Entities;

namespace VkBank.Application.Interfaces.Repositories
{
    public interface IMenuRepository : IGenericRepository<Menu>
    {
        public void CreateMenu(Menu menu);
        public void UpdateMenu(Menu menu);
        public void DeleteMenu(long id);
    }
}
