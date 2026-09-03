using Microsoft.AspNetCore.Mvc;
using VueAppAdmin.Server.Features.ExampleCategories.Responses;
using VueAppAdmin.Server.Shared;

namespace VueAppAdmin.Server.Features.ExampleCategories;

[ApiController]
[Route("api/[controller]")]
public class ExampleCategoriesController(IExampleCategoriesService categoriesService) : ControllerBase
{
    // POST api/ExampleCategories — 取得所有類別清單
    // 使用 POST 以便未來擴充篩選條件時不需更改 HTTP method
    [HttpPost]
    public IActionResult GetAll()
    {
        var categories = categoriesService.GetAll();
        return Ok(ApiResponse<ExampleCategoryResponse>.OkList(categories));
    }
}
