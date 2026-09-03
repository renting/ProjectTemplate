## Purpose

規範 demo 範本的 API 文件產生方式：.NET 內建 OpenAPI（3.1）+ Scalar UI，且不使用 Swashbuckle。

## Requirements

### Requirement: 使用 .NET 內建 OpenAPI 產生文件，不使用 Swashbuckle
後端 SHALL 使用 `Microsoft.AspNetCore.OpenApi`（`builder.Services.AddOpenApi()`）產生 API 文件，SHALL NOT 參考 `Swashbuckle.AspNetCore`，亦不得保留 `AddEndpointsApiExplorer()`（內建 OpenAPI 不需要）。

#### Scenario: 專案不含 Swashbuckle 參考
- **WHEN** 檢視 `VueApp1.Server.csproj`
- **THEN** 存在 `Microsoft.AspNetCore.OpenApi` 的 `PackageReference`，且不存在任何 `Swashbuckle.*` 參考

---

### Requirement: 以 Scalar 作為 API 文件 UI 且僅於 Development 開放
後端 SHALL 參考 `Scalar.AspNetCore`，並於 `app.Environment.IsDevelopment()` 為真時同時掛載 `app.MapOpenApi()` 與 `app.MapScalarApiReference()`；`launchSettings.json` 所有 profile 的 `launchUrl` SHALL 為 `scalar`。

#### Scenario: 文件與 UI 皆可存取
- **WHEN** 以 Development 環境啟動後端
- **THEN** `GET /openapi/v1.json` 回應 HTTP 200 且 `openapi` 欄位為 `3.1.x`，`GET /scalar/v1` 回應 HTTP 200

#### Scenario: 既有 WeatherForecast 端點列於文件中
- **WHEN** 請求 `GET /openapi/v1.json`
- **THEN** `paths` 包含 `/WeatherForecast`

#### Scenario: 啟動時自動開啟 Scalar
- **WHEN** 檢視 `VueApp1.Server/Properties/launchSettings.json`
- **THEN** 三個 profile 的 `launchUrl` 皆為 `scalar`
