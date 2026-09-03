using Microsoft.AspNetCore.Mvc;
using VueAppAdmin.Server.Features.FeatureList.Responses;
using VueAppAdmin.Server.Shared;

namespace VueAppAdmin.Server.Features.FeatureList;

[ApiController]
[Route("api/[controller]")]
public class FeaturesController(IFeaturesService featuresService) : ControllerBase
{
    // GET api/Features — 取得系統所有功能識別字清單
    // 供管理介面顯示「可分配功能」使用；一般使用者只需要 JWT claims 中的自身功能
    [HttpGet]
    public IActionResult GetAll()
    {
        var features = featuresService.GetAll();
        return Ok(ApiResponse<FeatureResponse>.OkList(features));
    }
}
