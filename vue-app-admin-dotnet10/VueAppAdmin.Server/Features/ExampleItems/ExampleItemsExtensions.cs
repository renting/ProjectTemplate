namespace VueAppAdmin.Server.Features.ExampleItems;

public static class ExampleItemsExtensions
{
    // 將 ExampleItems feature 的相依注入集中於此
    public static IServiceCollection AddExampleItemsFeature(this IServiceCollection services)
    {
        services.AddScoped<IExampleItemsService, ExampleItemsService>();

        return services;
    }
}
