using DbScriptExporter;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

if (args.Length < 3)
{
    Console.WriteLine("用法: DbScriptExporter <ServerName> <DatabaseName> <輸出資料夾> [UserId] [Password]");
    Console.WriteLine("  未提供 UserId 時，使用 Windows 整合式驗證 (LoginSecure)。");
    return 1;
}

var serverName = args[0];
var databaseName = args[1];
var outputFolder = args[2];
var userId = args.Length > 3 ? args[3] : null;
var password = args.Length > 4 ? args[4] : string.Empty;

var connection = new ServerConnection(serverName)
{
    LoginSecure = string.IsNullOrEmpty(userId),
};

if (!connection.LoginSecure)
{
    connection.Login = userId;
    connection.Password = password;
}

Directory.CreateDirectory(outputFolder);

var server = new Server(connection);
var database = server.Databases[databaseName];

if (database is null)
{
    Console.WriteLine($"[ERROR] 找不到資料庫 '{databaseName}'（伺服器：'{serverName}'）。");
    return 1;
}

Console.WriteLine($"連線至 '{serverName}'，資料庫 '{databaseName}'，驗證方式：{(connection.LoginSecure ? "Windows 整合式驗證" : "SQL Server 驗證")}");
Console.WriteLine($"輸出資料夾：{Path.GetFullPath(outputFolder)}");
Console.WriteLine();

var exporter = new DatabaseScriptExporter(server, outputFolder);
var summary = exporter.Export(database);

Console.WriteLine();
Console.WriteLine("===== 匯出完成 =====");
Console.WriteLine($"總物件數：{summary.TotalCount}");
Console.WriteLine($"成功：{summary.SuccessCount}");
Console.WriteLine($"失敗：{summary.FailureCount}");
Console.WriteLine($"輸出資料夾：{Path.GetFullPath(outputFolder)}");

return summary.FailureCount > 0 ? 1 : 0;
