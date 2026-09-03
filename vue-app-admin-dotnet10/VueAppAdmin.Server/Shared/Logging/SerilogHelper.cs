using Serilog;
using System.Collections.Concurrent;

namespace VueAppAdmin.Server.Shared.Logging;

public static class SerilogHelper
{
    private static readonly ConcurrentDictionary<string, Serilog.ILogger> _categoryLoggers = new();

    // 在 Program.cs 最頂端呼叫，設定 Log.Logger（靜態 logger）
    // 用於 DI 建立前的 bootstrap log 及應用程式崩潰時的 Log.Fatal
    // 寫入 logs/log-.txt；保留天數 365 天（此為 bootstrap logger，無法讀取設定檔）
    public static void Initialize()
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 365)
            .CreateLogger();

        Log.Logger = logger;
    }

    // 取得指定分類的獨立 logger，寫入 logs/{category}/{category}-.txt
    // 適用場景：排程任務、外部整合（SMS/Email/Push）、稽核 log 等需要獨立隔離的情況
    // 一般 Service / Controller 請使用 DI 注入的 ILogger<T>，不需要此方法
    public static Serilog.ILogger GetLogger<T>()
        => GetLogger(typeof(T).Name);

    public static Serilog.ILogger GetLogger(string category)
    {
        return _categoryLoggers.GetOrAdd(category, name =>
            new LoggerConfiguration()
                .WriteTo.File($"logs/{name}/{name}-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 365)
                .CreateLogger());
    }
}
