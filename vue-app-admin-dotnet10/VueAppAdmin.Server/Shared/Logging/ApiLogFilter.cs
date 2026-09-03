using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using VueAppAdmin.Server.Shared;

namespace VueAppAdmin.Server.Shared.Logging;

public class ApiLogFilter : IActionFilter, IAlwaysRunResultFilter
{
    private static readonly Serilog.ILogger _logger = SerilogHelper.GetLogger<ApiLogFilter>();

    private const string RequestKey = "ApiLog.Request";
    private const string StartTickKey = "ApiLog.StartTick";
    internal const string ValidationErrorsKey = "ApiLog.ValidationErrors";

    // 正常流程：暫存 masked request 與起始時間
    // TODO: 未來若要寫入 Basic_Api_Log.ModulesNames，此值須透過 ActionDescriptor
    // （context.ActionDescriptor as ControllerActionDescriptor 的 ControllerTypeInfo + ActionName）
    // 自動組出 "Namespace.Class/Method" 格式，禁止在各 Controller/Action 手動硬編碼字串。
    public void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Items[StartTickKey] = Environment.TickCount64;
        var requestObj = context.ActionArguments.Values.FirstOrDefault();
        context.HttpContext.Items[RequestKey] = MaskRequest(requestObj);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    // 401/400 短路時 OnActionExecuting 未執行，此處補設起始時間
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (!context.HttpContext.Items.ContainsKey(StartTickKey))
            context.HttpContext.Items[StartTickKey] = Environment.TickCount64;
    }

    // 所有案例（正常、401、400）都在這裡寫 log
    public void OnResultExecuted(ResultExecutedContext context)
    {
        var httpContext = context.HttpContext;

        var elapsedMs = httpContext.Items[StartTickKey] is long startTick
            ? Environment.TickCount64 - startTick
            : -1L;

        var user = httpContext.User?.FindFirstValue(ClaimTypes.Name) ?? "-";
        var statusCode = httpContext.Response.StatusCode;
        var requestData = httpContext.Items[RequestKey];

        object? responseData;
        if (httpContext.Items.TryGetValue(ValidationErrorsKey, out var validationErrors))
            responseData = validationErrors;
        else if (context.Result is ObjectResult { Value: IApiResponse apiResp })
            responseData = apiResp.ToLogSummary();
        else
            responseData = null;

        // RequestId 對應 HttpContext.TraceIdentifier，讓正常路徑（此處）與例外路徑
        // （ExceptionHandlingMiddleware）寫出的 log 可用同一個值互相對應
        var requestId = httpContext.TraceIdentifier;

        if (statusCode is 400 or 401 or 403)
            _logger.Warning(
                "[API] {Method} {Path} | user:{User} | {StatusCode} | req:{@Request} | res:{@Response} | {ElapsedMs}ms | reqId:{RequestId}",
                httpContext.Request.Method, httpContext.Request.Path, user, statusCode, requestData, responseData, elapsedMs, requestId);
        else
            _logger.Information(
                "[API] {Method} {Path} | user:{User} | {StatusCode} | req:{@Request} | res:{@Response} | {ElapsedMs}ms | reqId:{RequestId}",
                httpContext.Request.Method, httpContext.Request.Path, user, statusCode, requestData, responseData, elapsedMs, requestId);
    }

    // 用 reflection 將標記 [LogMask] 的欄位替換為 "***"
    private static object? MaskRequest(object? requestObj)
    {
        if (requestObj is null) return null;

        var properties = requestObj.GetType().GetProperties();
        var dict = new Dictionary<string, object?>(properties.Length);

        foreach (var prop in properties)
        {
            dict[prop.Name] = prop.IsDefined(typeof(LogMaskAttribute), false)
                ? "***"
                : prop.GetValue(requestObj);
        }
        return dict;
    }
}
