using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Domain.Common;
using VkBank.Domain.Entities;

namespace VkBank.Domain.Entities
{
    public class Menu : EntityBase
    {
        /// <summary>
        /// Identifier of the parent menu item. NULL if it is the parent menu item.
        /// </summary>
        public ushort? ParentId { get; set; }

        /// <summary>
        /// Name of the menu item
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Screen code associated with the menu item. Must start with 10000.
        /// </summary>
        public uint ScreenCode { get; set; }

        /// <summary>
        /// Type of the menu item (e.g., home menu = 1, profile menu = 2)
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Priority of the menu item for ordering purposes
        /// </summary>
        public byte Priority { get; set; }

        /// <summary>
        /// Keyword associated with the menu item for searching
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// Path to the icon associated with the menu item
        /// </summary>
        public string? IconPath { get; set; }

        /// <summary>
        /// Indicates whether the menu item is new
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Start date for the new menu item
        /// </summary>
        public DateTime NewStartDate { get; set; }

        /// <summary>
        /// End date for the new menu item
        /// </summary>
        public DateTime NewEndDate { get; set; }





        //Navigation property
        //[DapperIgnore]
        //public ICollection<Product> Products { get; set; }
    }
}
