using VueAppAdmin.Server.Shared.Logging;

namespace VueAppAdmin.Server.Features.Auth;

// TODO: 此服務使用 hardcode 帳密驗證，實際專案應改為查詢資料庫（透過 UserRepository）
// 參考 UserRepository.cs 中已預留但尚未啟用的 SQL 查詢範例
public class AuthService : IAuthService
{
    private static readonly Serilog.ILogger _logger = SerilogHelper.GetLogger<AuthService>();

    public bool ValidateCredentials(string username, string password)
    {
        _logger.Information("Login attempt for {Username}", username);

        // TODO: 替換為資料庫查詢
        // 正式版本應先取得使用者的 PasswordHash，再用 BCrypt 驗證
        var knownUsers = new[] { "admin", "viewer" };
        var result = knownUsers.Contains(username) && password == "password";

        // TODO: 登入紀錄存表 — 將登入嘗試（帳號、結果、IP、時間）寫入 LoginLogs 資料表，供稽核查閱
        return result;
    }

    // TODO: 替換為資料庫查詢（UserRepository.FindDisplayNameByUsername）
    public string GetUserDisplayName(string username)
        => username == "admin" ? "Administrator" : username;

    public string[] GetUserGroups(string username)
        => GroupFeatureStore.GetGroups(username);

    public string[] GetUserFeatures(string username)
        => GroupFeatureStore.GetFeatures(username);
}
