using System.ComponentModel.DataAnnotations;

namespace VueAppAdmin.Server.Shared.Jwt;

// 對應 appsettings.json 的 "Jwt" 區段
public class JwtOptions
{
    public const string SectionName = "Jwt";

    // Jwt:Issuer — Token 的發行者識別字，驗證時比對
    [Required]
    public string Issuer { get; set; } = string.Empty;

    // Jwt:SignKey — 簽章金鑰，至少 32 字元；正式環境請替換為強隨機值並透過 Secret 管理
    [Required]
    public string SignKey { get; set; } = string.Empty;

    // Jwt:TokenExpirationHours — Token 有效時數，預設 8 小時
    public int TokenExpirationHours { get; set; } = 8;
}
