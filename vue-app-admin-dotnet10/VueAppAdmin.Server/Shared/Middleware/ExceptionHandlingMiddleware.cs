using VueAppAdmin.Server.Shared;

namespace VueAppAdmin.Server.Shared.Middleware;

// 全域例外攔截中介軟體：捕捉所有未處理的例外，
// 統一回傳 HTTP 500 + ApiResponse 格式，避免原始堆疊追蹤洩漏給前端
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // RequestId 對應 HttpContext.TraceIdentifier，與 ApiLogFilter 正常路徑
            // 寫出的 log 共用同一個值，兩條路徑的 log 才能互相對應
            logger.LogError(ex, "Unhandled exception {Method} {Path} | reqId:{RequestId}",
                context.Request.Method, context.Request.Path, context.TraceIdentifier);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail("系統錯誤"));
        }
    }
}
