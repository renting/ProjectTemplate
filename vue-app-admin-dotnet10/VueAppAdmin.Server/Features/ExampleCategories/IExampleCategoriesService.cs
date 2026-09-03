using VueAppAdmin.Server.Features.ExampleCategories.Responses;

namespace VueAppAdmin.Server.Features.ExampleCategories;

public interface IExampleCategoriesService
{
    IEnumerable<ExampleCategoryResponse> GetAll();
}
