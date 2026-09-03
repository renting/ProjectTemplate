namespace VueAppAdmin.Server.Shared;

// 分頁版回應格式，繼承 ApiResponse<T> 並加上總筆數
// Total 供前端計算總頁數，Results 為當頁資料
public class ApiPagedResponse<T> : ApiResponse<T>
{
    public int Total { get; set; }

    public static ApiPagedResponse<T> OkPaged(IEnumerable<T> results, int total)
        => new() { Success = true, Results = results, Total = total };

    // 分頁資料量大，log 只記摘要；完整 items 待未來 ApiLogs 存表時保存
    public override object ToLogSummary()
        => new { Success, Total, Count = Results?.Count() ?? 0 };
}
