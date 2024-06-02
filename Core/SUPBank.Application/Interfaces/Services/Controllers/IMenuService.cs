using SUPBank.Domain.Entities;

namespace SUPBank.Application.Interfaces.Services.Controllers
{
    public interface IMenuService
    {
        /// <summary>
        /// Generates recursive menus based on the provided list of menus.
        /// </summary>
        /// <param name="menus">The list of menus.</param>
        /// <returns>The generated recursive menus.</returns>
        List<EntityMenu> RecursiveMenus(List<EntityMenu> menus);

        /// <summary>
        /// Generates a recursive menu by its ID from the provided list of menus.
        /// </summary>
        /// <param name="menus">The list of menus.</param>
        /// <param name="menuId">The ID of the menu.</param>
        /// <returns>The generated recursive menu.</returns>
        EntityMenu RecursiveMenu(List<EntityMenu> menus, long menuId);

        /// <summary>
        /// Filters the recursive menu by its ID along with its submenus from the provided recursive menus.
        /// </summary>
        /// <param name="recursiveMenus">The list of recursive menus.</param>
        /// <param name="menuId">The ID of the menu to filter.</param>
        /// <returns>The filtered recursive menu with its submenus that match the specified menu ID.</returns>
        EntityMenu? FilterRecursiveMenuByIdWithSubMenus(List<EntityMenu> recursiveMenus, long menuId);

        /// <summary>
        /// Filters the recursive menu by its ID from the provided recursive menus.
        /// </summary>
        /// <param name="recursiveMenus">The list of recursive menus.</param>
        /// <param name="menuId">The ID of the menu to filter.</param>
        /// <returns>The filtered recursive menu that matches the specified menu ID.</returns>
        EntityMenu? FilterRecursiveMenuById(List<EntityMenu> recursiveMenus, long menuId);

        /// <summary>
        /// Filters the recursive menus by a keyword from the provided recursive menus.
        /// </summary>
        /// <param name="recursiveMenus">The list of recursive menus.</param>
        /// <param name="keyword">The keyword to filter the menus by.</param>
        /// <returns>The filtered recursive menus that match the specified keyword.</returns>
        List<EntityMenu> FilterRecursiveMenusByKeyword(List<EntityMenu> recursiveMenus, string keyword);
    }
}
