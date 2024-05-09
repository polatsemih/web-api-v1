using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Domain.Common;
using VkBank.Domain.Entities;

namespace VkBank.Domain.Entities
{
    public class Category : EntityBase
    {
        public required ushort ParentId { get; set; }
        public required string Name { get; set; }
        public required uint ScreenCode { get; set; } // starts with 10000
        public required byte Type { get; set; } // home menu, profile manu
        public required byte Priority { get; set; } // order
        public required string Keyword { get; set; } // for searching
        public required string IconPath { get; set; }
        public required bool IsNew { get; set; } = true;
        public required DateTime NewStartDate { get; set; } = DateTime.Now;
        public required DateTime NewEndDate { get; set; } = DateTime.Now.AddDays(7);
    }
}
