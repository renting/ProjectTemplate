namespace VueAppAdmin.Server.Features.Menu;

public interface IMenuService
{
    IEnumerable<MenuNode> GetFilteredMenu(IEnumerable<string> features);
}
