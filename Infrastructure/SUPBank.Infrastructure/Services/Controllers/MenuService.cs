using SUPBank.Application.Interfaces.Services.Controllers;
using SUPBank.Domain.Entities;

namespace SUPBank.Infrastructure.Services.Controllers
{
    public class MenuService : IMenuService
    {
        public List<EntityMenu> RecursiveMenus(List<EntityMenu> menus)
        {
            var menuDictionary = menus.ToDictionary(menu => menu.Id);
            foreach (var menu in menus)
            {
                if (menu.ParentId != 0 && menuDictionary.TryGetValue(menu.ParentId, out var parentMenu))
                {
                    parentMenu.SubMenus.Add(menu);
                }
            }
            return menus.Where(menu => menu.ParentId == 0).ToList();
        }

        public EntityMenu RecursiveMenu(List<EntityMenu> menus, long menuId)
        {
            var menuDictionary = menus.ToDictionary(menu => menu.Id);
            foreach (var menu in menus)
            {
                if (menu.ParentId != 0 && menuDictionary.TryGetValue(menu.ParentId, out var parentMenu))
                {
                    parentMenu.SubMenus.Add(menu);
                }
            }
            return menus.First(menu => menu.Id == menuId);
        }

        public EntityMenu? FilterRecursiveMenuByIdWithSubMenus(List<EntityMenu> recursiveMenus, long menuId)
        {
            foreach (var recursiveMenu in recursiveMenus)
            {
                if (recursiveMenu.Id == menuId)
                {
                    return recursiveMenu;
                }

                if (recursiveMenu.SubMenus != null)
                {
                    var recursiveMenuSubMenu = FilterRecursiveMenuByIdWithSubMenus(recursiveMenu.SubMenus, menuId);
                    if (recursiveMenuSubMenu != null)
                    {
                        return recursiveMenuSubMenu;
                    }
                }
            }
            return default;
        }

        public EntityMenu? FilterRecursiveMenuById(List<EntityMenu> recursiveMenus, long menuId)
        {
            foreach (var recursiveMenu in recursiveMenus)
            {
                if (recursiveMenu.Id == menuId)
                {
                    return new()
                    {
                        Id = recursiveMenu.Id,
                        ParentId = recursiveMenu.ParentId,
                        Name_TR = recursiveMenu.Name_TR,
                        Name_EN = recursiveMenu.Name_EN,
                        ScreenCode = recursiveMenu.ScreenCode,
                        WebURL = recursiveMenu.WebURL,
                        Type = recursiveMenu.Type,
                        Priority = recursiveMenu.Priority,
                        IsSearch = recursiveMenu.IsSearch,
                        Keyword = recursiveMenu.Keyword,
                        Authority = recursiveMenu.Authority,
                        Icon = recursiveMenu.Icon,
                        IsGroup = recursiveMenu.IsGroup,
                        IsNew = recursiveMenu.IsNew,
                        NewStartDate = recursiveMenu.NewStartDate,
                        NewEndDate = recursiveMenu.NewEndDate,
                        IsActive = recursiveMenu.IsActive,
                        CreatedDate = recursiveMenu.CreatedDate,
                        LastModifiedDate = recursiveMenu.LastModifiedDate
                    };
                }

                if (recursiveMenu.SubMenus != null)
                {
                    var recursiveMenuSubMenu = FilterRecursiveMenuById(recursiveMenu.SubMenus, menuId);
                    if (recursiveMenuSubMenu != null)
                    {
                        return new()
                        {
                            Id = recursiveMenuSubMenu.Id,
                            ParentId = recursiveMenuSubMenu.ParentId,
                            Name_TR = recursiveMenuSubMenu.Name_TR,
                            Name_EN = recursiveMenuSubMenu.Name_EN,
                            ScreenCode = recursiveMenuSubMenu.ScreenCode,
                            WebURL = recursiveMenuSubMenu.WebURL,
                            Type = recursiveMenuSubMenu.Type,
                            Priority = recursiveMenuSubMenu.Priority,
                            IsSearch = recursiveMenuSubMenu.IsSearch,
                            Keyword = recursiveMenuSubMenu.Keyword,
                            Authority = recursiveMenuSubMenu.Authority,
                            Icon = recursiveMenuSubMenu.Icon,
                            IsGroup = recursiveMenuSubMenu.IsGroup,
                            IsNew = recursiveMenuSubMenu.IsNew,
                            NewStartDate = recursiveMenuSubMenu.NewStartDate,
                            NewEndDate = recursiveMenuSubMenu.NewEndDate,
                            IsActive = recursiveMenuSubMenu.IsActive,
                            CreatedDate = recursiveMenuSubMenu.CreatedDate,
                            LastModifiedDate = recursiveMenuSubMenu.LastModifiedDate
                        };
                    }
                }
            }
            return default;
        }

        public List<EntityMenu> FilterRecursiveMenusByKeyword(List<EntityMenu> recursiveMenus, string keyword)
        {
            List<EntityMenu> filteredRecursiveMenus = [];

            foreach (var recursiveMenu in recursiveMenus)
            {
                if (recursiveMenu.Keyword != null && recursiveMenu.Keyword.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    filteredRecursiveMenus.Add(recursiveMenu);
                }

                if (recursiveMenu.SubMenus != null)
                {
                    var recursiveMenuSubMenus = FilterRecursiveMenusByKeyword(recursiveMenu.SubMenus, keyword);
                    if (recursiveMenuSubMenus.Count != 0)
                    {
                        filteredRecursiveMenus.AddRange(recursiveMenuSubMenus);
                    }
                }
            }

            var filteredMenusWithoutSubMenus = filteredRecursiveMenus.Select(filteredMenu => new EntityMenu
            {
                Id = filteredMenu.Id,
                ParentId = filteredMenu.ParentId,
                Name_TR = filteredMenu.Name_TR,
                Name_EN = filteredMenu.Name_EN,
                ScreenCode = filteredMenu.ScreenCode,
                WebURL = filteredMenu.WebURL,
                Type = filteredMenu.Type,
                Priority = filteredMenu.Priority,
                IsSearch = filteredMenu.IsSearch,
                Keyword = filteredMenu.Keyword,
                Authority = filteredMenu.Authority,
                Icon = filteredMenu.Icon,
                IsGroup = filteredMenu.IsGroup,
                IsNew = filteredMenu.IsNew,
                NewStartDate = filteredMenu.NewStartDate,
                NewEndDate = filteredMenu.NewEndDate,
                IsActive = filteredMenu.IsActive,
                CreatedDate = filteredMenu.CreatedDate,
                LastModifiedDate = filteredMenu.LastModifiedDate
            }).ToList();

            return filteredMenusWithoutSubMenus.OrderBy(menu => menu.Id).ToList();
        }
    }
}
