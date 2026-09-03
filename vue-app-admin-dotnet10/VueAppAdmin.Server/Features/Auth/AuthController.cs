using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VueAppAdmin.Server.Features.Auth.Requests;
using VueAppAdmin.Server.Features.Auth.Responses;
using VueAppAdmin.Server.Shared;
using VueAppAdmin.Server.Shared.Jwt;

namespace VueAppAdmin.Server.Features.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IJwtService jwtService) : ControllerBase
{
    // POST api/Auth/Login — 驗證帳密並回傳 JWT Token
    // [AllowAnonymous] 覆蓋 Program.cs 全域的 AuthorizeFilter，讓未登入者可呼叫
    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!authService.ValidateCredentials(request.Username, request.Password))
            return Unauthorized(ApiResponse<object>.Fail("帳號或密碼錯誤"));

        var displayName = authService.GetUserDisplayName(request.Username);
        var features = authService.GetUserFeatures(request.Username);
        var token = jwtService.GenerateToken(request.Username, displayName, features);

        return Ok(ApiResponse<LoginResponse>.Ok(new LoginResponse { Token = token }, "登入成功"));
    }

    // GET api/Auth/Me — 從 JWT claims 取得目前登入使用者的詳細資訊
    // features 存放於 claims 中（逗號分隔字串），此處解析還原為 List
    // groups 則即時查詢（不存入 claims），避免群組異動後 Token 內容過時
    [HttpGet("Me")]
    public IActionResult Me()
    {
        var username = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var displayName = User.FindFirstValue("displayName") ?? string.Empty;
        var featuresStr = User.FindFirstValue("features") ?? string.Empty;
        var features = featuresStr.Length > 0
            ? featuresStr.Split(',').ToList()
            : [];
        var groups = authService.GetUserGroups(username).ToList();

        return Ok(ApiResponse<MeResponse>.Ok(new MeResponse
        {
            Username = username,
            DisplayName = displayName,
            Groups = groups,
            Features = features
        }));
    }
}
