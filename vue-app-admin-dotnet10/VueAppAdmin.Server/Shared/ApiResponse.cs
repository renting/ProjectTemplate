namespace VueAppAdmin.Server.Shared;

public interface IApiResponse
{
    object ToLogSummary();
}

// 統一 API 回應格式，所有 Controller 回傳值均包在此型別內
// Result 與 Results 擇一使用：單筆資料用 Result，清單用 Results
public class ApiResponse<T> : IApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    // 單筆結果
    public T? Result { get; set; }
    // 多筆結果集合
    public IEnumerable<T>? Results { get; set; }

    public static ApiResponse<T> Ok(T result, string? message = null)
        => new() { Success = true, Result = result, Message = message };

    public static ApiResponse<T> OkList(IEnumerable<T> results, string? message = null)
        => new() { Success = true, Results = results, Message = message };

    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Message = message };

    // TODO: 當「API Log 存表」功能實作時，改為精簡格式（Success、Message、筆數），完整 payload 由 ApiLogs 資料表保存
    public virtual object ToLogSummary() => this;
}
