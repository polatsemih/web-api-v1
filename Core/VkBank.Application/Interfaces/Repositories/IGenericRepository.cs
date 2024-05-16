using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Domain.Common;

namespace VkBank.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : EntityBase
    {
        List<T> GetAll();
        T GetById(long id);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
