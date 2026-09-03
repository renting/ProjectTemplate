namespace VueAppAdmin.Server.Features.Menu;

public static class MenuExtensions
{
    // 將 Menu feature 的相依注入集中於此
    public static IServiceCollection AddMenuFeature(this IServiceCollection services)
    {
        services.AddScoped<IMenuService, MenuService>();
        return services;
    }
}
