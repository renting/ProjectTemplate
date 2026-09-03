## Why

上游 repo [littlehorseboy/dotnet-new-templates](https://github.com/littlehorseboy/dotnet-new-templates) 的兩個範本目標框架為 .NET 8（測試專案甚至是 `net9.0`，版本不一致），API 文件仍依賴 `Swashbuckle.AspNetCore`。

.NET 9 起官方 Web API 範本已移除 Swashbuckle，改用 SDK 內建的 `Microsoft.AspNetCore.OpenApi`；繼續沿用 Swashbuckle 會讓範本與官方預設漸行漸遠，新進成員拿到的骨架與 `dotnet new webapi` 產出的結構不一致。

本次要在不改變前端與業務邏輯的前提下，產出一組完整對應 .NET 10 的範本，並與 .NET 8 版並存不衝突。

## What Changes

- **目標框架統一**：所有 `.csproj` 一律 `net10.0`（原本 `net8.0` / `net9.0` 混用）
- **API 文件改用內建 OpenAPI**：移除 `Swashbuckle.AspNetCore`，改用 `Microsoft.AspNetCore.OpenApi`（OpenAPI 3.1），UI 改用 `Scalar.AspNetCore`
- **JWT security scheme 改寫**：Swashbuckle 的 `AddSecurityDefinition` / `AddSecurityRequirement` 改以 `IOpenApiDocumentTransformer` 實作（新檔 `Shared/OpenApi/BearerSecuritySchemeTransformer.cs`）
- **`launchSettings.json`**：三個 profile 的 `launchUrl` 由 `swagger` 改為 `scalar`
- **套件版本更新**：JwtBearer / SpaProxy → `10.*`；測試相依（Test.Sdk、xunit、NSubstitute、coverlet）與 SqlClient、SMO 升到現行版本
- **demo 範本方案檔改 `.slnx`**：與 admin 範本一致；連帶移除 `template.json` 已失效的 `guids` 設定
- **openspec 規格結構修正**：capability 目錄由巢狀（`vue-app-admin-dotnet8/<cap>/`）改為 CLI 支援的單層扁平 id（`admin-<cap>` / `demo-<cap>`），每份 spec 補上 `## Purpose` + `## Requirements` 標頭，使 `openspec list --specs` 與 `openspec validate --all` 實際可用
- **範本識別更新**：short name 加 `-dotnet10` 後綴、identity 更名、port range 改到 171xx–185xx、新增 `sdk-version` 約束 `[10.0-*)`

## Capabilities

### New Capabilities

- `target-framework`：跨兩個範本的目標框架與 ASP.NET Core 相依版本約束
- `admin-api-documentation`：內建 OpenAPI + Scalar 的文件產生與 JWT security scheme 行為
- `demo-api-documentation`：demo 範本的文件產生與 UI 行為

### Modified Capabilities

- `solution-startup-order`：由「`.sln` 的 project 宣告順序」推廣為「兩個範本一律使用 `.slnx`，Server 排在 client 之前」
- `admin-template-config`：short name / identity / port range 更新、新增 SDK 約束、移除已不存在的 `.vscode/launch.json` 需求、port 影響檔案清單修正
- `demo-template-config`：安裝路徑改為目錄名、方案檔改 `.slnx`、移除 `guids` 需求、補上 port 隨機化與 postActions 需求
- `admin-backend-architecture`：修正 ExampleItems 端點敘述（實際為 `POST /Search` + `GET /{id:int}`，非 `GET /api/ExampleItems`）
- `admin-backend-auth`：修正 JWT 擴充方法的檔名（`JwtServiceCollectionExtensions.cs` → `Shared/Jwt/JwtExtensions.cs`），並清除中段殘留的 delta 標頭
- `admin-logging`：清除中段殘留的 delta 標頭（原本使其後 11 條 requirement 不被 CLI 解析）

### Removed Capabilities

- `admin-solution-format`（原 `vue-app-admin-dotnet8/solution-format`）：內容併入 `solution-startup-order`，且原本「`template.json` 的 `guids` 應包含方案 GUID」的需求對 `.slnx` 不成立

## Impact

- **`vue-app-admin-dotnet10/`**：`Program.cs`、三個 `.csproj`、`launchSettings.json`、`Shared/OpenApi/`（新增）、`.template.config/template.json`、`VueAppAdmin.Server/README.md`
- **`vue-app-demo-dotnet10/`**：`Program.cs`、`.csproj`、`launchSettings.json`、`VueApp1.sln` → `VueApp1.slnx`、`.template.config/template.json`
- **`openspec/`**：`config.yaml` 原樣沿用；`specs/` 承接並改寫為 .NET 10 現況（25 個 capability、127 條 requirement）；`changes/archive/` 僅保留本次移植一筆
- **`.claude/skills/`**：複製上游 5 個 openspec skill（內容無 repo 專屬字串，原樣沿用）
- **根目錄 `README.md`**：重寫，新增 .NET 8 → .NET 10 變更對照表與 `Microsoft.OpenApi` v2 命名空間提醒
- 不影響前端程式碼（Vue / Vite / PrimeVue 版本與設定原封不動）
- 不影響認證邏輯、API 合約、log 行為、db schema
- 與 .NET 8 版範本可同機並存（short name、identity、port range 皆錯開）
