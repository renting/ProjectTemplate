## ADDED Requirements

### Requirement: 使用 .NET 內建 OpenAPI 產生文件，不使用 Swashbuckle
後端 SHALL 使用 `Microsoft.AspNetCore.OpenApi`（`builder.Services.AddOpenApi()`）產生 API 文件，SHALL NOT 參考 `Swashbuckle.AspNetCore`。

#### Scenario: 專案不含 Swashbuckle 參考
- **WHEN** 檢視 `VueAppAdmin.Server.csproj`
- **THEN** 存在 `Microsoft.AspNetCore.OpenApi` 的 `PackageReference`，且不存在任何 `Swashbuckle.*` 參考

#### Scenario: 程式碼不含 Swagger API 呼叫
- **WHEN** 於 `Program.cs` 全文搜尋 `AddSwaggerGen`、`UseSwagger`、`UseSwaggerUI`
- **THEN** 無任何符合結果

---

### Requirement: 文件端點僅於 Development 環境開放
`app.MapOpenApi()` 與 Scalar UI SHALL 僅在 `app.Environment.IsDevelopment()` 為真時掛載。

#### Scenario: Development 環境可取得文件
- **WHEN** 以 `ASPNETCORE_ENVIRONMENT=Development` 啟動後端並請求 `GET /openapi/v1.json`
- **THEN** 回應 HTTP 200，body 為合法 JSON，`openapi` 欄位值為 `3.1.x`

#### Scenario: Production 環境不開放文件
- **WHEN** 以 `ASPNETCORE_ENVIRONMENT=Production` 啟動後端並請求 `GET /openapi/v1.json`
- **THEN** 不回應 200（文件端點未掛載）

---

### Requirement: 以 Scalar 作為 API 文件 UI
後端 SHALL 參考 `Scalar.AspNetCore` 並呼叫 `app.MapScalarApiReference()` 提供互動式 API 文件介面；`launchSettings.json` 的所有 profile 的 `launchUrl` SHALL 為 `scalar`，不得為 `swagger`。

#### Scenario: Scalar UI 可存取
- **WHEN** 以 Development 環境啟動後端並請求 `GET /scalar/v1`
- **THEN** 回應 HTTP 200，回傳 Scalar 文件介面的 HTML

#### Scenario: 啟動時自動開啟 Scalar
- **WHEN** 檢視 `VueAppAdmin.Server/Properties/launchSettings.json`
- **THEN** `http`、`https`、`IIS Express` 三個 profile 的 `launchUrl` 皆為 `scalar`

---

### Requirement: 以 document transformer 補上 JWT Bearer security scheme
因內建 OpenAPI 不提供 Swashbuckle 的 `AddSecurityDefinition` / `AddSecurityRequirement`，專案 SHALL 於 `Shared/OpenApi/BearerSecuritySchemeTransformer.cs` 實作 `IOpenApiDocumentTransformer`，並以 `options.AddDocumentTransformer<BearerSecuritySchemeTransformer>()` 註冊。

transformer SHALL：
- 於 `components.securitySchemes` 加入名稱為 `Bearer` 的 scheme（`type: http`、`scheme: bearer`、`bearerFormat: JWT`、`in: header`）
- 於文件根層 `security` 加入對應的全域 security requirement，對齊 `Program.cs` 全域套用的 `AuthorizeFilter`
- 在專案未註冊名為 `Bearer` 的 authentication scheme 時不修改文件，避免產出誤導性的 security scheme

#### Scenario: 文件包含 Bearer security scheme
- **WHEN** 請求 `GET /openapi/v1.json`
- **THEN** `components.securitySchemes.Bearer` 存在，其 `type` 為 `http`、`scheme` 為 `bearer`、`bearerFormat` 為 `JWT`

#### Scenario: 文件包含全域 security requirement
- **WHEN** 請求 `GET /openapi/v1.json`
- **THEN** 根層 `security` 陣列包含一筆以 `Bearer` 為 key 的 requirement

#### Scenario: Scalar 可帶入 token 呼叫受保護端點
- **WHEN** 使用者於 Scalar UI 填入由 `POST /api/Auth/Login` 取得的 JWT 後呼叫任一受保護端點
- **THEN** 請求帶有 `Authorization: Bearer <token>` 標頭，回應非 401

---

### Requirement: transformer 使用 Microsoft.OpenApi v2 命名空間
.NET 10 隨附的 `Microsoft.OpenApi` 為 v2，`OpenApiDocument`、`OpenApiSecurityScheme`、`OpenApiComponents` 等型別已由 `Microsoft.OpenApi.Models` 移至 `Microsoft.OpenApi`。transformer SHALL `using Microsoft.OpenApi;`，security requirement 的 key SHALL 使用 `OpenApiSecuritySchemeReference`。

#### Scenario: 使用舊命名空間會編譯失敗
- **WHEN** 將 transformer 的 using 改為 `Microsoft.OpenApi.Models` 並建置
- **THEN** 編譯失敗（CS0234：命名空間 `Microsoft.OpenApi` 中沒有類型或命名空間名稱 `Models`）

#### Scenario: 現行程式碼可正常建置
- **WHEN** 執行 `dotnet build`
- **THEN** 建置成功，0 錯誤 0 警告
