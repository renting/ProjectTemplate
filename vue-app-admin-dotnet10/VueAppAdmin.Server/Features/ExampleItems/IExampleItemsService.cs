using VueAppAdmin.Server.Features.ExampleItems.Requests;
using VueAppAdmin.Server.Features.ExampleItems.Responses;

namespace VueAppAdmin.Server.Features.ExampleItems;

public interface IExampleItemsService
{
    ItemResponse? GetById(int id);
    (IEnumerable<ItemResponse> Items, int Total) Search(ExampleItemsSearchRequest request);
}
