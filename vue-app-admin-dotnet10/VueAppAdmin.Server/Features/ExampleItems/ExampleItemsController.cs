using Microsoft.AspNetCore.Mvc;
using VueAppAdmin.Server.Features.ExampleItems.Requests;
using VueAppAdmin.Server.Features.ExampleItems.Responses;
using VueAppAdmin.Server.Shared;

namespace VueAppAdmin.Server.Features.ExampleItems;

[ApiController]
[Route("api/[controller]")]
public class ExampleItemsController(IExampleItemsService exampleItemsService) : ControllerBase
{
    // POST api/ExampleItems/Search — 分頁搜尋，請求參數包含頁碼、每頁筆數、排序與篩選條件
    // 使用 POST 而非 GET，因為篩選條件可能包含陣列（CategoryIds）
    [HttpPost("Search")]
    public IActionResult Search([FromBody] ExampleItemsSearchRequest request)
    {
        var (items, total) = exampleItemsService.Search(request);
        return Ok(ApiPagedResponse<ItemResponse>.OkPaged(items, total));
    }

    // GET api/ExampleItems/{id} — 依 ID 取得單筆項目
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var item = exampleItemsService.GetById(id);
        return item is null
            ? NotFound(ApiResponse<object>.Fail("找不到指定項目"))
            : Ok(ApiResponse<ItemResponse>.Ok(item));
    }
}
