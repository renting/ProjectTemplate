namespace VueAppAdmin.Server.Features.ExampleItems.Requests;

public class ExampleItemsSearchRequest
{
    // 第幾頁（從 1 開始）
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    // 排序欄位名稱（id / name / description）
    public string SortField { get; set; } = "id";
    // 排序方向（asc / desc）
    public string SortOrder { get; set; } = "asc";
    public string? Name { get; set; }
    public string? Description { get; set; }
    // 多選類別過濾；空清單表示不限類別
    public List<int> CategoryIds { get; set; } = [];
    // CreatedDate 查詢區間（含首尾兩端點）；僅帶一端代表開放區間
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
