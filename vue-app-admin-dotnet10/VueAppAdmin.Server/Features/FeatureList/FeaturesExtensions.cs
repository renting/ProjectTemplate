namespace VueAppAdmin.Server.Features.FeatureList;

public static class FeaturesExtensions
{
    // 將 FeatureList feature 的相依注入集中於此
    public static IServiceCollection AddFeaturesFeature(this IServiceCollection services)
    {
        services.AddScoped<IFeaturesService, FeaturesService>();
        return services;
    }
}
