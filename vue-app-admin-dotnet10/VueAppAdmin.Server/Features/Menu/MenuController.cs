using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VueAppAdmin.Server.Shared;

namespace VueAppAdmin.Server.Features.Menu;

[ApiController]
[Route("api/[controller]")]
public class MenuController(IMenuService menuService) : ControllerBase
{
    // POST api/Menu/Items — 依目前登入使用者的功能權限，回傳過濾後的選單節點清單
    // features 從 JWT claims 取出（逗號分隔字串），傳給 MenuService 進行過濾
    [HttpPost("Items")]
    public IActionResult GetItems()
    {
        var featuresStr = User.FindFirstValue("features") ?? string.Empty;
        var features = featuresStr.Length > 0
            ? featuresStr.Split(',')
            : [];

        var nodes = menuService.GetFilteredMenu(features);
        return Ok(ApiResponse<MenuNode>.OkList(nodes));
    }
}
