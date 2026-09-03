using VueAppAdmin.Server.Features.ExampleItems.Requests;
using VueAppAdmin.Server.Features.ExampleItems.Responses;

namespace VueAppAdmin.Server.Features.ExampleItems;

// TODO: 此服務使用記憶體靜態資料（30 筆），實際專案應改為資料庫查詢
public class ExampleItemsService : IExampleItemsService
{
    // demo 用類別資料，與 ExampleCategoriesService 的資料對應
    private static readonly (int Id, string Name)[] _categories =
    [
        (1, "A 類"), (2, "B 類"), (3, "C 類")
    ];

    // 產生 30 筆 demo 資料，類別依 id % 3 循環分配
    // CreatedDate 用固定的 DateTime.Today.AddDays(-i) 遞減，避免用 DateTime.Now 造成資料每次啟動飄移、測試不穩定
    private static readonly List<ItemResponse> _items = Enumerable.Range(1, 30).Select(i =>
    {
        var catId = (i % 3) + 1;
        var catName = _categories.First(c => c.Id == catId).Name;
        return new ItemResponse
        {
            Id = i,
            Name = $"Item {i:D2}",
            Description = $"Description for item number {i}.",
            CategoryId = catId,
            CategoryName = catName,
            CreatedDate = DateTime.Today.AddDays(-i)
        };
    }).ToList();

    public ItemResponse? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    // 在記憶體中執行篩選、排序後再分頁
    // 實際專案應將篩選排序轉為 SQL（或 ORM 查詢），避免全表載入
    //
    // TODO: 未來改用 SQL Server + Dapper 時的參考寫法（此處僅為註解範例，不影響現行 in-memory 實作）。
    // 重點 1：排序欄位不可把使用者傳入的字串直接拼進 SQL 的 ORDER BY（SQL Injection 風險），
    //        需先經白名單字典映射成實際 SQL 欄位名稱，再組進 SQL 文字。
    // 重點 2：分頁資料與 Total 筆數用 Dapper 的 QueryMultipleAsync 在同一次往返中一起取得，
    //        避免兩次查詢造成的額外網路延遲與資料不一致風險。
    // 重點 3：CategoryName 排序在真實 SQL 中需要 JOIN Categories 表，
    //        與目前 in-memory 版本（CategoryName 已是 ItemResponse 既有屬性）成本不同。
    //
    // private static readonly Dictionary<string, string> SortColumnMap = new(StringComparer.OrdinalIgnoreCase)
    // {
    //     ["id"] = "i.Id",
    //     ["name"] = "i.Name",
    //     ["description"] = "i.Description",
    //     ["categoryName"] = "c.Name",
    //     ["createdDate"] = "i.CreatedDate",
    // };
    //
    // var sortColumn = SortColumnMap.GetValueOrDefault(request.SortField, "i.Id");
    // var sortDirection = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
    //
    // const string filterSql = @"
    //     WHERE (@Name IS NULL OR i.Name LIKE '%' + @Name + '%')
    //       AND (@Description IS NULL OR i.Description LIKE '%' + @Description + '%')
    //       AND (@DateFrom IS NULL OR i.CreatedDate >= @DateFrom)
    //       AND (@DateTo IS NULL OR i.CreatedDate <= @DateTo)";
    //
    // var sql = $@"
    //     SELECT i.Id, i.Name, i.Description, i.CategoryId, c.Name AS CategoryName, i.CreatedDate
    //     FROM Items i
    //     INNER JOIN Categories c ON c.Id = i.CategoryId
    //     {filterSql}
    //     ORDER BY {sortColumn} {sortDirection}
    //     OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;
    //
    //     SELECT COUNT(*)
    //     FROM Items i
    //     {filterSql};
    // ";
    //
    // using var multi = await db.QueryMultipleAsync(sql, new
    // {
    //     request.Name,
    //     request.Description,
    //     request.DateFrom,
    //     request.DateTo,
    //     Skip = (request.Page - 1) * request.PageSize,
    //     request.PageSize
    // });
    //
    // var items = (await multi.ReadAsync<ItemResponse>()).AsList();
    // var total = await multi.ReadSingleAsync<int>();
    public (IEnumerable<ItemResponse> Items, int Total) Search(ExampleItemsSearchRequest request)
    {
        var query = _items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(x => x.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Description))
            query = query.Where(x => x.Description.Contains(request.Description, StringComparison.OrdinalIgnoreCase));

        if (request.CategoryIds.Count > 0)
            query = query.Where(x => request.CategoryIds.Contains(x.CategoryId));

        // CreatedDate 查詢區間為閉區間（含首尾兩端點）；僅帶一端代表開放區間
        if (request.DateFrom.HasValue)
            query = query.Where(x => x.CreatedDate >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(x => x.CreatedDate <= request.DateTo.Value);

        // 排序欄位預設為 id；不支援的欄位名稱一律 fallback 到 id 排序
        query = request.SortField.ToLowerInvariant() switch
        {
            "name" => request.SortOrder == "desc"
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name),
            "description" => request.SortOrder == "desc"
                ? query.OrderByDescending(x => x.Description)
                : query.OrderBy(x => x.Description),
            "categoryname" => request.SortOrder == "desc"
                ? query.OrderByDescending(x => x.CategoryName)
                : query.OrderBy(x => x.CategoryName),
            "createddate" => request.SortOrder == "desc"
                ? query.OrderByDescending(x => x.CreatedDate)
                : query.OrderBy(x => x.CreatedDate),
            _ => request.SortOrder == "desc"
                ? query.OrderByDescending(x => x.Id)
                : query.OrderBy(x => x.Id),
        };

        var filtered = query.ToList();
        var total = filtered.Count;
        var skip = (request.Page - 1) * request.PageSize;
        var items = filtered.Skip(skip).Take(request.PageSize);
        return (items, total);
    }
}
