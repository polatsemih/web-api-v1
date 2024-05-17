using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkBank.Domain.Contstants
{
    public class ResultMessages
    {
        //Menu
        public const string MenuCreated = "Menu Created";
        public const string MenuCreatedFailed = "Menu Creation Failed";
        public const string MenuUpdated = "Menu Updated";
        public const string MenuUpdatedFailed = "Menu Update Failed";
        public const string MenuDeleted = "Menu Deleted";
        public const string MenuDeletedFailed = "Menu Deleted Failed";

        public const string MenuNoDatas = "No Menu Datas Found";
        public const string MenuNoData = "No Menu Data Found By Given Id";
    }
}
