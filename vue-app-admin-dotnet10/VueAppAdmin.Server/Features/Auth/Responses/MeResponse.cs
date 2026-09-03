namespace VueAppAdmin.Server.Features.Auth.Responses;

public class MeResponse
{
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    // 使用者所屬群組（即時查詢，不存入 Token）
    public List<string> Groups { get; set; } = [];
    // 使用者擁有的功能識別字（從 JWT claims 解析）
    public List<string> Features { get; set; } = [];
}
