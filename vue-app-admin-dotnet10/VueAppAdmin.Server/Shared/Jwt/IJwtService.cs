namespace VueAppAdmin.Server.Shared.Jwt;

public interface IJwtService
{
    // 產生含使用者資訊與功能權限清單的 JWT Token
    // features 為逗號分隔的功能識別字字串，寫入 claims 後前端可解析
    string GenerateToken(string username, string displayName, string[] features);
}
