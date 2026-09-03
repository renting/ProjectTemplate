namespace VueAppAdmin.Server.Features.Auth;

// ============================================================
// !! DEMO 用 Hardcode 轉換表 !!
//
// 此類別以記憶體 Dictionary 模擬「使用者 → 群組 → 功能權限」的對應關係，
// 僅供範本展示用途。
//
// 實際專案 TODO：
//   1. 將使用者、群組、功能權限資料移至資料庫（對應資料表：Basic_Users_Groups、
//      Basic_Group_Modules、Basic_Modules，見 db/schema.sql）
//   2. 透過 IUserRepository / 群組 Repository 進行查詢
//   3. 刪除此靜態類別
//
// 對應 SQL 查詢（依 UserGuid 查出所有已授權功能，五種權限旗標中任一為 1 即視為擁有）：
//   const string sql = """
//       SELECT DISTINCT m.ModuleLink
//       FROM Basic_Users_Groups ug
//       JOIN Basic_Group_Modules gm ON gm.GroupId = ug.GroupId
//       JOIN Basic_Modules m ON m.ModuleId = gm.ModuleId
//       WHERE ug.UserGuid = @UserGuid
//         AND (gm.ViewStatus = 1 OR gm.AddStatus = 1 OR gm.EditStatus = 1
//              OR gm.DelStatus = 1 OR gm.DownloadStatus = 1)
//       """;
//   本範本的功能識別字（如 "items:read"）與 Basic_Modules.ModuleLink（如 "/example-items"）
//   命名方式不同，正式接 DB 時需選定一種對應規則（建議另建 ModuleFeatureCode 欄位或改用
//   ModuleLink 本身作為識別字，並同步調整 MenuService / FeaturesService）。
// ============================================================
public static class GroupFeatureStore
{
    // TODO: 替換為資料庫查詢 — 使用者所屬群組
    private static readonly Dictionary<string, string[]> _userGroups = new()
    {
        ["admin"] = ["SuperAdmins"],
        ["viewer"] = ["ReadOnly"]
    };

    // TODO: 替換為資料庫查詢 — 群組擁有的功能識別字
    // 功能識別字格式：<資源>:<動作>，例如 "items:read"、"categories:manage"
    private static readonly Dictionary<string, string[]> _groupFeatures = new()
    {
        ["SuperAdmins"] = ["items:read", "items:write", "categories:manage", "menu:admin"],
        ["ReadOnly"] = ["items:read"]
    };

    public static string[] GetGroups(string username)
        => _userGroups.TryGetValue(username, out var groups) ? groups : [];

    // 取得使用者所有群組的功能聯集（去重複）
    public static string[] GetFeatures(string username)
    {
        var groups = GetGroups(username);
        return [.. groups
            .SelectMany(g => _groupFeatures.TryGetValue(g, out var f) ? f : [])
            .Distinct()];
    }
}
