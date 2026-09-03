## 1. 專案骨架與目標框架

- [x] 1.1 由上游 repo 複製兩個範本目錄，更名為 `vue-app-admin-dotnet10` / `vue-app-demo-dotnet10`，一併帶入 `assets/` 與 `.gitignore`；不複製 `openspec/`、`prompts/`（見 design 決策 4）
- [x] 1.2 將四個 `.csproj` 的 `TargetFramework` 統一為 `net10.0`（原 Server 與 DbScriptExporter 為 `net8.0`、測試專案為 `net9.0`）
- [x] 1.3 更新套件版本：JwtBearer / SpaProxy → `10.*` / `10.*-*`；Test.Sdk 17.12→18.9、xunit 2.9.2→2.9.3、NSubstitute 5.3→6.2、coverlet 6.0.2→10.0.1、SqlClient 7.0.1→7.0.2、SMO 181.25→181.36。版本先以 nuget.org flatcontainer index 查證存在後才寫入
- [x] 1.4 `dotnet build` 驗證：admin Server、DbScriptExporter、demo Server 皆 0 錯誤 0 警告；`dotnet test` 23 項測試全數通過（`.NETCoreApp,Version=v10.0`）

## 2. API 文件改用內建 OpenAPI + Scalar

- [x] 2.1 admin `.csproj`：移除 `Swashbuckle.AspNetCore`，加入 `Microsoft.AspNetCore.OpenApi` `10.*` 與 `Scalar.AspNetCore` `2.17.2`
- [x] 2.2 新增 `VueAppAdmin.Server/Shared/OpenApi/BearerSecuritySchemeTransformer.cs`：實作 `IOpenApiDocumentTransformer`，注入 `IAuthenticationSchemeProvider` 確認 `Bearer` scheme 存在後才寫入 `components.securitySchemes` 與根層 `security`
- [x] 2.3 **實測踩坑並修正**：初版 `using Microsoft.OpenApi.Models;` 建置失敗（CS0234），確認 .NET 10 隨附的 `Microsoft.OpenApi` 為 v2、型別已移至 `Microsoft.OpenApi` 根命名空間；改用 `using Microsoft.OpenApi;` 後建置成功
- [x] 2.4 admin `Program.cs`：`AddEndpointsApiExplorer()` + `AddSwaggerGen(...)` 換成 `AddOpenApi(o => o.AddDocumentTransformer<BearerSecuritySchemeTransformer>())`；`UseSwagger()` / `UseSwaggerUI()` 換成 `MapOpenApi()` + `MapScalarApiReference(o => o.WithTitle("VueAppAdmin API"))`，維持僅 Development 掛載
- [x] 2.5 demo `Program.cs` / `.csproj` 做同樣替換（無 JWT，故不需 transformer）
- [x] 2.6 兩個範本的 `launchSettings.json` 三個 profile 的 `launchUrl` 由 `swagger` 改為 `scalar`
- [x] 2.7 **執行時驗證**（Development、`--no-launch-profile`）：`/openapi/v1.json` 回 200 且 `openapi` 為 `3.1.1`；`components.securitySchemes.Bearer` 與根層 `security` 皆正確產出；`/scalar/v1` 回 200；`POST /api/auth/login` 回傳 token；無 token 呼叫 `POST /api/Menu/Items` 回 401。demo 的 `/openapi/v1.json`、`/scalar/v1`、`/weatherforecast` 皆 200

## 3. demo 範本方案檔改 .slnx

- [x] 3.1 `dotnet sln VueApp1.sln migrate` 產生 `VueApp1.slnx` 後刪除 `.sln`
- [x] 3.2 手動調整 `.slnx`：Server `.csproj` 排到 client `.esproj` 之前（migrate 產出順序相反，違反 `solution-startup-order`），並移除 migrate 帶出的 `<Build />` / `<Deploy />`
- [x] 3.3 移除 `template.json` 的 `guids` 陣列（`.slnx` 不含 GUID；且上游那三個 GUID 本就與 `VueApp1.sln` 實際值不符，從未生效）
- [x] 3.4 修正兩處 README 中殘留的 `.sln` 字樣（`VueAppAdmin.Server/README.md`、`DbScriptExporter/README.md`）
- [x] 3.5 `dotnet build VueApp1.slnx` 驗證成功

## 4. 範本設定（template.json）

- [x] 4.1 admin：`identity` → `VueAppAdminDotnet10.CSharp`、`shortName` → `vue-app-admin-dotnet10`、`description` 改述 .NET 10 與 OpenAPI/Scalar；port range 161xx–165xx → 171xx–175xx
- [x] 4.2 demo：`identity` → `VueAppDemoDotnet10.CSharp`、`shortName` → `vue-app-demo-dotnet10`；port range 151xx–155xx → 181xx–185xx；補上 `postActions`（上游 demo 沒有）
- [x] 4.3 兩者新增 `constraints.sdk-version: "[10.0-*)"`，確認加入後 `dotnet new install` 仍成功且範本正常列於 `dotnet new list`
- [x] 4.4 admin `postActions` 指引文字補上 API 文件位置（`/scalar`、`/openapi/v1.json`）
- [x] 4.5 兩份 `template.json` 以 `JSON.parse` 驗證語法合法

## 5. 範本產生端到端驗證

- [x] 5.1 `dotnet new install` 兩個範本皆成功，`dotnet new list` 可見
- [x] 5.2 `dotnet new vue-app-admin-dotnet10 -n MyAdminApp`：檔名/命名空間/小寫目錄替換正確，全文搜尋無 `VueAppAdmin` 殘留；port 落在 171xx–175xx 且同目錄內一致；`Jwt:SignKey` 為 `MyAdminApp-31bc6e846cda47609d523313f385ebdf`（專案名 + 32 hex）
- [x] 5.3 產出專案 `dotnet build MyAdminApp.slnx` 成功、`dotnet test` 23 項通過；執行時 `/openapi/v1.json` 標題為 `MyAdminApp.Server | v1`、securityScheme 正確、`/scalar/v1` 200、登入成功
- [x] 5.4 `dotnet new vue-app-demo-dotnet10 -n MyDemoApp`：產出 `MyDemoApp.slnx`；`dotnet build` 成功
- [x] 5.5 清理：移除驗證過程產生的 `bin`/`obj`/`node_modules`/`logs`/`package-lock.json`，比對檔案清單確認範本目錄相對上游只多出 `BearerSecuritySchemeTransformer.cs` 一個檔案

## 6. openspec 工作流搬遷與規格改寫

- [x] 6.1 複製 `.claude/skills/openspec-*`（5 個 skill，經 grep 確認無 repo 專屬內容）與 `openspec/config.yaml`（繁中撰寫規範，原樣沿用）；確認 `openspec` CLI 已安裝（v1.2.0）
- [x] 6.2 **實測發現上游規格結構 CLI 不認**：`openspec list --specs` 對巢狀的 `vue-app-admin-dotnet8/<cap>/spec.md` 只列出頂層目錄且 `requirements 0`，`openspec validate --all` 全數失敗（缺 `## Purpose` / `## Requirements` 標頭）——上游的驗證機制等同從未生效，規格漂移因此無人察覺
- [x] 6.3 capability 目錄扁平化為 CLI 支援的單層 id：`vue-app-admin-dotnet8/<cap>` → `admin-<cap>`、`vue-app-demo/<cap>` → `demo-<cap>`、`sln-startup-order` → `solution-startup-order`
- [x] 6.4 每份 spec 由 delta 格式（`## ADDED Requirements`）改為主規格格式：補上 `## Purpose`（逐一撰寫該 capability 的用途）與 `## Requirements` 標頭，requirement / scenario 內容不動
- [x] 6.5 新增 `target-framework`、`admin-api-documentation`、`demo-api-documentation` 三個 capability
- [x] 6.6 改寫 `solution-startup-order`：由 `.sln` 宣告順序推廣為「兩範本一律 `.slnx`、Server 在前、不宣告 `guids`」；刪除 `admin-solution-format`（內容併入，且其 GUID 需求對 `.slnx` 不成立）
- [x] 6.7 改寫兩份 template-config：port 表改為實際值、移除不存在的 `.vscode/launch.json` 需求、補上 SDK 約束與 postActions/JwtSecret 需求、修正 port 影響檔案清單（實際含 `README.md` 與 `vite.config.ts`）
- [x] 6.8 修正 `admin-backend-architecture` 的 ExampleItems 端點漂移（`GET /api/ExampleItems` → `POST /api/ExampleItems/Search` + `GET /{id:int}`），核對 `ApiPagedResponse` 實際欄位為 `results` / `total` 後才寫入
- [x] 6.9 以小工具比對所有 spec 中反引號標注的檔名與範本目錄實際檔案，抓出 `backend-auth` 引用的 `JwtServiceCollectionExtensions.cs` 不存在（實際為 `Shared/Jwt/JwtExtensions.cs`），已修正
- [x] 6.10 清除 `admin-backend-auth` 與 `admin-logging` 檔案中段殘留的 `## ADDED Requirements` 標頭（上游併檔遺留），該標頭原本會讓其後的 requirement 全部不被 CLI 解析；清除後可解析 requirement 由 115 條增為 127 條
- [x] 6.11 併入上游未歸檔但實作已完成的 `add-db-schema-and-template-ux`：新增 `admin-db-schema` capability、`admin-data-access` 補上 TODO SQL 需求、`admin-template-config` 補上 JwtSecret / postActions / db 資料夾需求
- [x] 6.12 以本次移植建立 `openspec/changes/archive/2026-09-03-port-templates-to-dotnet10/`，作為本 repo 工作流的第一筆歷史；change 內的 `specs/` 維持 delta 格式（`## ADDED` / `## MODIFIED` / `## REMOVED`）
- [x] 6.13 `openspec validate --all` 通過：25 passed / 0 failed；`openspec list --specs` 正確列出 25 個 capability 共 127 條 requirement

## 7. 文件

- [x] 7.1 重寫根目錄 `README.md`：範本清單、安裝方式、兩範本說明、.NET 8 → .NET 10 變更對照表、`Microsoft.OpenApi` v2 命名空間提醒、維護筆記
- [x] 7.2 更新 `VueAppAdmin.Server/README.md`：Swagger UI 段落改為 Scalar + OpenAPI 兩個端點，目錄樹補上 `Shared/OpenApi/`
- [x] 7.3 README 補上 openspec 工作流的使用說明
