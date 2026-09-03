namespace VueAppAdmin.Server.Features.Menu;

// 選單樹狀節點，支援多層巢狀結構
public class MenuNode
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    // Bootstrap Icons class name，例如 "bi-speedometer2"
    public string Icon { get; set; } = string.Empty;
    // 前端路由路徑；null 表示此節點為群組（資料夾），不可點擊
    public string? Route { get; set; }
    // 必須擁有的功能識別字；null 表示所有登入使用者均可見
    public string? RequiredFeature { get; set; }
    public List<MenuNode> Children { get; set; } = [];
}
