using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VkBank.Domain.Common;

namespace VkBank.Domain.Entities
{
    public class Menu : EntityBase
    {
        /// <summary>
        /// Identifier of the parent menu item. NULL if it is the parent menu item.
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// Turkish name of the menu item
        /// </summary>
        public string Name_TR { get; set; }

        /// <summary>
        /// English name of the menu item
        /// </summary>
        public required string Name_EN { get; set; }

        /// <summary>
        /// Screen code associated with the menu item. Must start with 10000.
        /// </summary>
        public int ScreenCode { get; set; }

        /// <summary>
        /// Type of the menu item (e.g., my profile menu = 10, my world menu = 20, all transactions = 30)
        /// </summary>
        public required byte Type { get; set; }

        /// <summary>
        /// Priority of the menu item for ordering purposes
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// Keyword associated with the menu item for searching
        /// </summary>
        public required string Keyword { get; set; }

        /// <summary>
        /// Path to the icon associated with the menu item
        /// </summary>
        public byte? Icon { get; set; }

        /// <summary>
        /// Indicates whether the menu item is a group
        /// </summary>
        public bool IsGroup { get; set; }

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
    }
}
