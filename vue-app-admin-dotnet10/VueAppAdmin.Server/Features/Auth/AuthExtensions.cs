namespace VueAppAdmin.Server.Features.Auth;

public static class AuthExtensions
{
    // 將 Auth feature 的相依注入集中於此，Program.cs 只需呼叫 AddAuthFeature()
    public static IServiceCollection AddAuthFeature(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
