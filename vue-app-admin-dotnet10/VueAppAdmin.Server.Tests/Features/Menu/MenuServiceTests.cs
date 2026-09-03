using VueAppAdmin.Server.Features.Menu;

namespace VueAppAdmin.Server.Tests.Features.Menu;

// MenuService 直接 new()，不需 mock
// 重點測試：features 權限過濾後的選單節點是否正確
public class MenuServiceTests
{
    private readonly MenuService _sut = new();

    [Fact]
    public void GetFilteredMenu_AdminFeatures_ReturnsFullTree()
    {
        // admin 擁有所有功能，應看到完整選單（3 個頂層節點）
        var features = new[] { "items:read", "items:write", "categories:manage", "menu:admin" };
        var nodes = _sut.GetFilteredMenu(features).ToList();

        Assert.Equal(3, nodes.Count);
        var dataManagement = nodes.First(n => n.Route == null && n.Label == "資料管理");
        Assert.Equal(2, dataManagement.Children.Count);
    }

    [Fact]
    public void GetFilteredMenu_ViewerFeatures_FiltersRestrictedNodes()
    {
        // viewer 只有 items:read，系統管理應被過濾；資料管理只剩範例清單
        var features = new[] { "items:read" };
        var nodes = _sut.GetFilteredMenu(features).ToList();

        Assert.DoesNotContain(nodes, n => n.Label == "系統管理");
        var dataManagement = nodes.FirstOrDefault(n => n.Label == "資料管理");
        Assert.NotNull(dataManagement);
        Assert.Single(dataManagement.Children);
        Assert.Equal("範例清單", dataManagement.Children[0].Label);
    }

    [Fact]
    public void GetFilteredMenu_NoFeatures_HidesGroupNodeWhenAllChildrenFiltered()
    {
        // 無功能時，資料管理與系統管理（群組節點）子節點全被過濾 → 群組本身也應隱藏
        var features = Array.Empty<string>();
        var nodes = _sut.GetFilteredMenu(features).ToList();

        Assert.DoesNotContain(nodes, n => n.Label == "資料管理");
        Assert.DoesNotContain(nodes, n => n.Label == "系統管理");
        Assert.Contains(nodes, n => n.Label == "儀表板");
    }
}
