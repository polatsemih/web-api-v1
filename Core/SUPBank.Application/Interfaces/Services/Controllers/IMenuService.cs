using SUPBank.Domain.Entities;

namespace SUPBank.Application.Interfaces.Services.Controllers
{
    public interface IMenuService
    {
        List<EntityMenu> RecursiveMenu(List<EntityMenu> menus);
        EntityMenu? FilterMenuById(List<EntityMenu> menus, long id);
        List<EntityMenu> FilterMenusByKeyword(List<EntityMenu> menus, string keyword);
    }
}
