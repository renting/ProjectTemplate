using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace VueAppAdmin.Server.Shared.Jwt;

public static class JwtExtensions
{
    // 設定 JWT Bearer 驗證：繫結 JwtOptions、設定 Token 驗證參數，並注入 IJwtService
    // ValidateAudience = false：此範本不限定 audience，如需限定請自行設定 ValidAudiences
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 繫結設定並在啟動時驗證（Issuer、SignKey 為必填）
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SignKey))
                };
            });

        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
