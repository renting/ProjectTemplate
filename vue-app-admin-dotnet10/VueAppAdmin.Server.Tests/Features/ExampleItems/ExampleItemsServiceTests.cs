using VueAppAdmin.Server.Features.ExampleItems;
using VueAppAdmin.Server.Features.ExampleItems.Requests;

namespace VueAppAdmin.Server.Tests.Features.ExampleItems;

// ExampleItemsService 直接 new()，不需 mock（記憶體資料，無外部相依）
public class ExampleItemsServiceTests
{
    private readonly ExampleItemsService _sut = new();

    [Fact]
    public void Search_NoFilters_ReturnsFirstPageOf10()
    {
        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 10 };
        var (items, total) = _sut.Search(request);

        // 共 30 筆 demo 資料，第一頁應回傳 10 筆
        Assert.Equal(30, total);
        Assert.Equal(10, items.Count());
    }

    [Fact]
    public void Search_ByName_ReturnsMatchingItems()
    {
        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, Name = "Item 1" };
        var (items, _) = _sut.Search(request);

        Assert.All(items, i => Assert.Contains("Item 1", i.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Search_ByDescription_ReturnsMatchingItems()
    {
        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, Description = "number 5" };
        var (items, _) = _sut.Search(request);

        Assert.All(items, i => Assert.Contains("number 5", i.Description, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Search_ByCategoryIds_ReturnsOnlyMatchingCategories()
    {
        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, CategoryIds = [1] };
        var (items, _) = _sut.Search(request);

        Assert.All(items, i => Assert.Equal(1, i.CategoryId));
    }

    [Fact]
    public void Search_MultipleCategories_ReturnsAllMatchingItems()
    {
        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, CategoryIds = [1, 2] };
        var (items, _) = _sut.Search(request);

        Assert.All(items, i => Assert.True(i.CategoryId == 1 || i.CategoryId == 2));
    }

    [Fact]
    public void Search_SortByCategoryNameDesc_ReturnsDescendingOrder()
    {
        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, SortField = "categoryName", SortOrder = "desc" };
        var (items, _) = _sut.Search(request);

        var names = items.Select(i => i.CategoryName).ToList();
        Assert.Equal(names.OrderByDescending(n => n, StringComparer.Ordinal), names);
    }

    [Fact]
    public void Search_SortByCreatedDateDesc_ReturnsNewestFirst()
    {
        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, SortField = "createdDate", SortOrder = "desc" };
        var (items, _) = _sut.Search(request);

        var dates = items.Select(i => i.CreatedDate).ToList();
        Assert.Equal(dates.OrderByDescending(d => d), dates);
    }

    [Fact]
    public void Search_ByDateRange_ReturnsOnlyItemsWithinRange()
    {
        var probe = new ExampleItemsSearchRequest { Page = 1, PageSize = 30 };
        var (allItems, _) = _sut.Search(probe);
        var ordered = allItems.OrderBy(i => i.CreatedDate).ToList();
        var from = ordered[5].CreatedDate;
        var to = ordered[10].CreatedDate;

        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, DateFrom = from, DateTo = to };
        var (items, _) = _sut.Search(request);

        Assert.All(items, i => Assert.InRange(i.CreatedDate, from, to));
        Assert.Equal(6, items.Count());
    }

    [Fact]
    public void Search_DateFromOnly_ReturnsItemsOnOrAfterDate()
    {
        var probe = new ExampleItemsSearchRequest { Page = 1, PageSize = 30 };
        var (allItems, _) = _sut.Search(probe);
        var from = allItems.OrderBy(i => i.CreatedDate).ElementAt(10).CreatedDate;

        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, DateFrom = from };
        var (items, _) = _sut.Search(request);

        Assert.All(items, i => Assert.True(i.CreatedDate >= from));
    }

    [Fact]
    public void Search_DateToOnly_ReturnsItemsOnOrBeforeDate()
    {
        var probe = new ExampleItemsSearchRequest { Page = 1, PageSize = 30 };
        var (allItems, _) = _sut.Search(probe);
        var to = allItems.OrderBy(i => i.CreatedDate).ElementAt(10).CreatedDate;

        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, DateTo = to };
        var (items, _) = _sut.Search(request);

        Assert.All(items, i => Assert.True(i.CreatedDate <= to));
    }

    [Fact]
    public void Search_DateRangeWithNameFilter_ReturnsItemsMatchingBoth()
    {
        var probe = new ExampleItemsSearchRequest { Page = 1, PageSize = 30 };
        var (allItems, _) = _sut.Search(probe);
        var ordered = allItems.OrderBy(i => i.CreatedDate).ToList();
        var from = ordered[0].CreatedDate;
        var to = ordered[^1].CreatedDate;

        var request = new ExampleItemsSearchRequest { Page = 1, PageSize = 30, Name = "Item 1", DateFrom = from, DateTo = to };
        var (items, _) = _sut.Search(request);

        Assert.All(items, i =>
        {
            Assert.Contains("Item 1", i.Name, StringComparison.OrdinalIgnoreCase);
            Assert.InRange(i.CreatedDate, from, to);
        });
    }

    [Fact]
    public void GetById_ExistingId_ReturnsItem()
    {
        var result = _sut.GetById(1);
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void GetById_NonExistingId_ReturnsNull()
    {
        var result = _sut.GetById(999);
        Assert.Null(result);
    }
}
