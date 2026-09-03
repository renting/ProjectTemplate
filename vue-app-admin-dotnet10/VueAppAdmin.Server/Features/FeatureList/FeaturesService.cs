using VueAppAdmin.Server.Features.FeatureList.Responses;

namespace VueAppAdmin.Server.Features.FeatureList;

// TODO: 此服務使用記憶體靜態清單，實際專案應改為資料庫查詢
// 功能識別字格式：<資源>:<動作>，與 GroupFeatureStore 及 MenuService 保持一致
public class FeaturesService : IFeaturesService
{
    private static readonly List<FeatureResponse> _features =
    [
        new() { Code = "items:read",        Description = "範例清單 - 讀取" },
        new() { Code = "items:write",       Description = "範例清單 - 寫入" },
        new() { Code = "categories:manage", Description = "類別管理" },
        new() { Code = "menu:admin",        Description = "系統管理選單" },
    ];

    public IEnumerable<FeatureResponse> GetAll() => _features;
}
