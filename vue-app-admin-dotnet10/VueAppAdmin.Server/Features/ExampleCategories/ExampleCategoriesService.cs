using VueAppAdmin.Server.Features.ExampleCategories.Responses;

namespace VueAppAdmin.Server.Features.ExampleCategories;

// TODO: 此服務使用記憶體靜態資料，實際專案應改為資料庫查詢
// 此處的類別 Id 與 ExampleItemsService 的分類保持一致
public class ExampleCategoriesService : IExampleCategoriesService
{
    private static readonly List<ExampleCategoryResponse> _categories =
    [
        new() { Id = 1, Name = "A 類" },
        new() { Id = 2, Name = "B 類" },
        new() { Id = 3, Name = "C 類" },
    ];

    public IEnumerable<ExampleCategoryResponse> GetAll() => _categories;
}
