using SUPBank.Application.Interfaces.Services.Controllers;
using SUPBank.Domain.Entities;

namespace SUPBank.Infrastructure.Services.Controller
{
    public class MenuService : IMenuService
    {
        public List<EntityMenu> RecursiveMenu(List<EntityMenu> menus)
        {
            var menuDictionary = menus.ToDictionary(menu => menu.Id);
            foreach (var menu in menus)
            {
                if (menu.ParentId != 0 && menuDictionary.TryGetValue(menu.ParentId, out var parentMenu))
                {
                    parentMenu.SubMenus.Add(menu);
                }
            }
            return menus;
        }

        public EntityMenu? FilterMenuById(List<EntityMenu> menus, long id)
        {
            foreach (var menu in menus)
            {
                if (menu.Id == id)
                {
                    return menu;
                }

                if (menu.SubMenus != null)
                {
                    var subMenu = FilterMenuById(menu.SubMenus, id);
                    if (subMenu != null)
                    {
                        return subMenu;
                    }
                }
            }
            return default;
        }

        public List<EntityMenu> FilterMenusByKeyword(List<EntityMenu> menus, string keyword)
        {
            List<EntityMenu> filteredMenus = [];

            foreach (var menu in menus)
            {
                if (menu.Keyword != null && menu.Keyword.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    filteredMenus.Add(menu);
                }

                if (menu.SubMenus != null)
                {
                    var subMenus = FilterMenusByKeyword(menu.SubMenus, keyword);
                    if (subMenus.Count != 0)
                    {
                        filteredMenus.AddRange(subMenus);
                    }
                }
            }

            return filteredMenus.OrderBy(menu => menu.Id).ToList();
        }
    }
}
