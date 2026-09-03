namespace VueAppAdmin.Server.Features.ExampleCategories;

public static class ExampleCategoriesExtensions
{
    // 將 ExampleCategories feature 的相依注入集中於此
    public static IServiceCollection AddExampleCategoriesFeature(this IServiceCollection services)
    {
        services.AddScoped<IExampleCategoriesService, ExampleCategoriesService>();
        return services;
    }
}
