using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace VueAppAdmin.Server.Shared.OpenApi;

/// <summary>
/// 在 .NET 10 內建的 OpenAPI 文件中補上 JWT Bearer 的 security scheme 與全域 security requirement，
/// 讓 Scalar / Swagger 等 UI 能顯示「Authorize」欄位並自動帶入 Authorization: Bearer &lt;token&gt; 標頭。
/// 取代 .NET 8 時期由 Swashbuckle 的 AddSecurityDefinition / AddSecurityRequirement 負責的工作。
/// </summary>
internal sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    private const string SchemeName = "Bearer";

    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var schemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        // 專案未註冊 Bearer 驗證時就不動文件，避免產出誤導性的 security scheme
        if (!schemes.Any(scheme => scheme.Name == SchemeName))
        {
            return;
        }

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "輸入由 /api/auth/login 取得的 JWT，UI 會自動加上 Bearer 前綴"
        };

        // 對應 Program.cs 全域套用的 AuthorizeFilter：預設所有端點都需要 token
        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SchemeName, document)] = []
        });
    }
}
