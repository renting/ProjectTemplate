using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Scalar.AspNetCore;
using Serilog;
using VueAppAdmin.Server.Features.Auth;
using VueAppAdmin.Server.Features.ExampleCategories;
using VueAppAdmin.Server.Features.ExampleItems;
using VueAppAdmin.Server.Features.FeatureList;
using VueAppAdmin.Server.Features.Menu;
using VueAppAdmin.Server.Shared;
using VueAppAdmin.Server.Shared.Database;
using VueAppAdmin.Server.Shared.Jwt;
using VueAppAdmin.Server.Shared.Logging;
using VueAppAdmin.Server.Shared.Middleware;
using VueAppAdmin.Server.Shared.OpenApi;

// bootstrap logger：在 DI 容器建立前即可記錄，捕捉啟動期間的錯誤
SerilogHelper.Initialize();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // 讀取 Log 保留天數設定（Logging:RetentionDays），預設 365 天
    var logRetentionDays = builder.Configuration.GetValue<int>("Logging:RetentionDays", 365);

    // system logger：記錄 HTTP 請求與應用程式層級事件，獨立於各 feature 的 logger
    // MinimumLevel 由 appsettings.json 的 Serilog:MinimumLevel 控制（Development 預設 Debug，Production 預設 Information）
    var systemLogger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console()
        .WriteTo.File("logs/log-system-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: logRetentionDays)
        .CreateLogger();

    builder.Services.AddSerilog(systemLogger);

    // 全域套用 [Authorize]，所有 Controller 預設需要驗證
    // 需要匿名存取的端點（如 Login）須加上 [AllowAnonymous]
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new AuthorizeFilter());
        options.Filters.Add(new ApiLogFilter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // 覆寫預設的 Model Validation 回應格式，統一為 ApiResponse
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

            // 供 ApiLogFilter 取用，統一由 filter 記錄 log
            context.HttpContext.Items[ApiLogFilter.ValidationErrorsKey] = errors;

            return new BadRequestObjectResult(ApiResponse<object>.Fail("驗證失敗"));
        };
    });

    // .NET 10 內建的 OpenAPI 文件產生器（輸出 OpenAPI 3.1），取代 .NET 8 時期的 Swashbuckle
    // JWT Bearer 的 security scheme 由 BearerSecuritySchemeTransformer 補上
    builder.Services.AddOpenApi(options =>
    {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    });

    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddDatabase(builder.Configuration);
    builder.Services.AddAuthFeature();
    builder.Services.AddFeaturesFeature();
    builder.Services.AddExampleCategoriesFeature();
    builder.Services.AddExampleItemsFeature();
    builder.Services.AddMenuFeature();

    var app = builder.Build();

    // 最先掛載例外攔截中介軟體，確保後續任何未處理例外都能被捕捉
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // 提供 Vue 前端靜態檔（由 Vite build 產生後放置於 wwwroot）
    app.UseDefaultFiles();
    app.UseStaticFiles();

    if (app.Environment.IsDevelopment())
    {
        // OpenAPI 文件：/openapi/v1.json
        app.MapOpenApi();
        // Scalar API 文件介面：/scalar，右上角 Authorize 可填入 JWT
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("VueAppAdmin API");
        });
    }

    app.UseHttpsRedirection();
    // 記錄每個 HTTP 請求的基本資訊（method、path、status、elapsed）
    app.UseSerilogRequestLogging();

    // 注意順序：Authentication 必須在 Authorization 之前
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // SPA fallback：所有非 API 的路徑都回傳 index.html，讓 Vue Router 接管
    app.MapFallbackToFile("/index.html");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
