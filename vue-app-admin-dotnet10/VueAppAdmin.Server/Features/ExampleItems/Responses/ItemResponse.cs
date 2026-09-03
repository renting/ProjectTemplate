namespace VueAppAdmin.Server.Features.ExampleItems.Responses;

public class ItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
