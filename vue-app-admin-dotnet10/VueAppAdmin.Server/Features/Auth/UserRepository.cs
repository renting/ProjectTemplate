using System.Data;

namespace VueAppAdmin.Server.Features.Auth;

// 使用者資料庫存取層（Dapper）
// 目前方法均為 TODO 骨架，待 AuthService 改為資料庫驗證後啟用
// 對應資料表：Basic_Users（見 db/schema.sql）
public class UserRepository(IDbConnection db) : IUserRepository
{
    private readonly IDbConnection _db = db;

    // 驗證使用者帳密
    // 注意：BCrypt 雜湊內嵌隨機 salt，無法在 SQL 端直接比對雜湊字串。
    // 正式實作應改為：(1) 依 NormalAccount 查出 PasswordHash，(2) 於應用程式端以
    // BCrypt.Verify(plainPassword, storedHash) 比對，而非把已雜湊的密碼傳入 SQL WHERE。
    // 下方 SQL 僅示範第一步查詢，第二步請在 AuthService 呼叫 BCrypt.Verify。
    public bool ExistsWithCredentials(string username, string hashedPassword)
    {
        // TODO: 替換為實際 SQL 查詢，並改為先取出 PasswordHash 再於程式端驗證
        // const string sql = """
        //     SELECT TOP 1 PasswordHash
        //     FROM Basic_Users
        //     WHERE NormalAccount = @Username
        //       AND Status = 1                                             -- 僅允許啟用帳號登入
        //       AND (PasswordUpdLockDateTime IS NULL OR PasswordUpdLockDateTime <= GETDATE())
        //     """;
        // var storedHash = db.QuerySingleOrDefault<string>(sql, new { Username = username });
        // return storedHash != null && BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);
        return false;
    }

    public string? FindDisplayNameByUsername(string username)
    {
        // TODO: 替換為實際 SQL 查詢
        // const string sql = "SELECT UserName FROM Basic_Users WHERE NormalAccount = @Username AND Status = 1";
        // return db.QuerySingleOrDefault<string>(sql, new { Username = username });
        return null;
    }
}
