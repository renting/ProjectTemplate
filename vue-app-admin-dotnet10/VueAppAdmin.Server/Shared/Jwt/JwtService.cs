using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VueAppAdmin.Server.Shared.Jwt;

public class JwtService(IOptions<JwtOptions> jwtOptions, ILogger<JwtService> logger) : IJwtService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateToken(string username, string displayName, string[] features)
    {
        // Claims 說明：
        //   ClaimTypes.Name  → 標準使用者名稱，供 User.Identity.Name 使用
        //   displayName      → 顯示名稱，前端 UI 顯示用
        //   features         → 逗號分隔的功能識別字，前端用來控制選單與按鈕可見性
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("displayName", displayName),
            new Claim("features", string.Join(",", features))
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SignKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Token 有效時數從 appsettings.json Jwt:TokenExpirationHours 讀取（預設 8 小時）
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtOptions.TokenExpirationHours),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        logger.LogInformation("Token generated for {Username}", username);

        return tokenString;
    }
}
