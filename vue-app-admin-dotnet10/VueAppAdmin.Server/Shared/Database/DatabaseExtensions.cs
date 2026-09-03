using Microsoft.Data.SqlClient;
using System.Data;

namespace VueAppAdmin.Server.Shared.Database;

public static class DatabaseExtensions
{
    // 以 Scoped 生命週期注入 IDbConnection（每次 HTTP 請求建立一個連線，請求結束自動釋放）
    // Service 透過建構子注入 IDbConnection，搭配 Dapper 執行 SQL
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDbConnection>(_ =>
            new SqlConnection(configuration.GetConnectionString("Default")));

        return services;
    }
}
