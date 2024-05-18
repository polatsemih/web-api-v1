using VkBank.Domain.Entities.Common;

namespace VkBank.Domain.Entities
{
    public class Menu : EntityBase
    {
        /// <summary>
        /// Identifier of the parent menu item. 0 if it is the top-level parent menu item.
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// Turkish name of the menu item
        /// </summary>
        public string Name_TR { get; set; }

        /// <summary>
        /// English name of the menu item
        /// </summary>
        public string Name_EN { get; set; }

        /// <summary>
        /// Screen code associated with the menu item. The code must be within the range starting from 10000.
        /// </summary>
        public int ScreenCode { get; set; }

        /// <summary>
        /// Type of the menu item (e.g., profile menu = 10, my world menu = 20, all transactions = 30)
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Priority of the menu item for ordering
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Keyword associated with the menu item for searching
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Path to the icon associated with the menu item
        /// </summary>
        public string? Icon { get; set; }

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
        public DateTime? NewStartDate { get; set; }

        /// <summary>
        /// End date for the new menu item
        /// </summary>
        public DateTime? NewEndDate { get; set; }


        public List<Menu> Children { get; set; } = new List<Menu>();
    }
}
