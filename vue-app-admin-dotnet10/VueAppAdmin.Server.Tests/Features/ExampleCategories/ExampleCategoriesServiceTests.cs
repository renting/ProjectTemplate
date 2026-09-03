using VueAppAdmin.Server.Features.ExampleCategories;

namespace VueAppAdmin.Server.Tests.Features.ExampleCategories;

// ExampleCategoriesService 直接 new()，不需 mock
public class ExampleCategoriesServiceTests
{
    private readonly ExampleCategoriesService _sut = new();

    [Fact]
    public void GetAll_ReturnsExactlyThreeCategories()
    {
        var result = _sut.GetAll().ToList();
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void GetAll_ContainsExpectedIds()
    {
        var ids = _sut.GetAll().Select(c => c.Id).ToList();
        Assert.Contains(1, ids);
        Assert.Contains(2, ids);
        Assert.Contains(3, ids);
    }
}
