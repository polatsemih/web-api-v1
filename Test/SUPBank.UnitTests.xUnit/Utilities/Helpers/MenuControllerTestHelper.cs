using Moq;
using SUPBank.Application.Interfaces.Services.Controllers;
using SUPBank.Domain.Entities;

namespace SUPBank.UnitTests.xUnit.Utilities.Helpers
{
    public static class MenuControllerTestHelper
    {
        public static void SetupMenuServiceRecursiveMenus(Mock<IMenuService> menuServiceMock, List<EntityMenu> mediatorMenus, List<EntityMenu> recursiveMenus)
        {
            menuServiceMock.Setup(m => m.RecursiveMenus(mediatorMenus))
                           .Returns(recursiveMenus);
        }

        public static void SetupMenuServiceRecursiveMenu(Mock<IMenuService> menuServiceMock, List<EntityMenu> mediatorMenus, long menuId, EntityMenu recursiveMenu)
        {
            menuServiceMock.Setup(m => m.RecursiveMenu(mediatorMenus, menuId))
                           .Returns(recursiveMenu);
        }

        public static void SetupMenuServiceFilterRecursiveMenuById(Mock<IMenuService> menuServiceMock, List<EntityMenu> menus, long menuId, EntityMenu filteredMenu)
        {
            menuServiceMock.Setup(m => m.FilterRecursiveMenuById(menus, menuId))
                           .Returns(filteredMenu);
        }

        public static void SetupMenuServiceFilterRecursiveMenuByIdWithSubMenus(Mock<IMenuService> menuServiceMock, List<EntityMenu> menus, long menuId, EntityMenu filteredMenu)
        {
            menuServiceMock.Setup(m => m.FilterRecursiveMenuByIdWithSubMenus(menus, menuId))
                           .Returns(filteredMenu);
        }

        public static void SetupMenuServiceFilterRecursiveMenusByKeyword(Mock<IMenuService> menuServiceMock, List<EntityMenu> menus, string keyword, List<EntityMenu> filteredMenus)
        {
            menuServiceMock.Setup(m => m.FilterRecursiveMenusByKeyword(menus, keyword))
                           .Returns(filteredMenus);
        }

        public static List<EntityMenu> GetMenusMock()
        {
            return
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1" },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", Keyword = "Keyword2" },
                new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3" },
                new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4" },
                new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5" }
            ];
        }

        public static List<EntityMenu> GetRecursiveMenusMock()
        {
            return
            [
                new() { Id = 1, ParentId = 0, Name_EN = "Menu1", Keyword = "Keyword1", SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", Keyword = "Keyword3", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", Keyword = "Keyword4", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", Keyword = "Keyword5", SubMenus = [] }
                    ] }
                ] },
                new() { Id = 2, ParentId = 0, Name_EN = "Menu2", Keyword = "Keyword2", SubMenus = [] }
            ];
        }

        public static EntityMenu GetMenuMock()
        {
            return new()
            {
                Id = 1,
                ParentId = 0,
                Name_EN = "Menu1"
            };
        }

        public static EntityMenu GetRecursiveMenuMock()
        {
            return new()
            {
                Id = 1,
                ParentId = 0,
                Name_EN = "Menu1",
                SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", SubMenus = [] }
                    ] }
                ]
            };
        }

        public static EntityMenu GetDynamicMenuMock(long id)
        {
            return new()
            {
                Id = id,
                ParentId = 0,
                Name_EN = "Menu1"
            };
        }

        public static EntityMenu GetDynamicRecursiveMenuMock(long id)
        {
            return new()
            {
                Id = id,
                ParentId = 0,
                Name_EN = "Menu1",
                SubMenus = [
                    new() { Id = 3, ParentId = 1, Name_EN = "SubMenu1", SubMenus = [] },
                    new() { Id = 4, ParentId = 1, Name_EN = "SubMenu2", SubMenus = [
                        new() { Id = 5, ParentId = 4, Name_EN = "SubSubMenu1", SubMenus = [] }
                    ] }
                ]
            };
        }
    }
}
