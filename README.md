# ProjectTemplate

自訂 `dotnet new` 專案範本集合，.NET 10 版本。

移植自 [littlehorseboy/dotnet-new-templates](https://github.com/littlehorseboy/dotnet-new-templates)（.NET 8 版本），
目標框架升級至 `net10.0`，並將 API 文件從 Swashbuckle 改為 .NET 內建的 `Microsoft.AspNetCore.OpenApi` + Scalar UI。

> 參考：[dotnet new 的自訂範本 — Microsoft Learn](https://learn.microsoft.com/zh-tw/dotnet/core/tools/custom-templates)

---

## 快速開始

### 前置需求

| 需求 | 版本 | 備註 |
|------|------|------|
| [.NET SDK](https://dotnet.microsoft.com/download/dotnet/10.0) | 10.0 以上 | 兩個範本都宣告了 `sdk-version` 約束（`[10.0-*)`），舊版 SDK 不會看到範本 |
| [Node.js](https://nodejs.org/) | `^20.19.0` 或 `>=22.12.0` | |
| [pnpm](https://pnpm.io/installation) | 任意版本 | **admin 範本需要**，其前端 dev server 由 SpaProxy 以 `pnpm run dev` 啟動；未安裝請執行 `npm i -g pnpm`。demo 範本用 npm，不需要 pnpm |

### 步驟 1：Clone 並安裝範本（每台機器只需做一次）

```bash
git clone https://github.com/renting/ProjectTemplate.git
cd ProjectTemplate
dotnet new install .\vue-app-admin-dotnet10
dotnet new install .\vue-app-demo-dotnet10
```

確認裝好了：

```bash
dotnet new list vue-app
```

應該看到兩筆：

```
ASP.NET Core + Vue 3 (Admin, .NET 10)   vue-app-admin-dotnet10   [C#]
ASP.NET Core + Vue 3 (Demo, .NET 10)    vue-app-demo-dotnet10    [C#]
```

> **不要刪掉或搬動 clone 下來的資料夾。** `dotnet new` 註冊的是「資料夾路徑」而非複製內容，資料夾一旦搬走，範本就會失效。搬動後需先 `dotnet new uninstall <舊路徑>` 再對新路徑重新 install。

### 步驟 2：建立新專案

切換到要放專案的目錄，擇一執行：

```bash
# 完整後台管理系統骨架（JWT 認證、Serilog、權限選單、DB schema、單元測試）
dotnet new vue-app-admin-dotnet10 -n MyApp

# 輕量前後端整合骨架（只有 Vue + Web API + OpenAPI）
dotnet new vue-app-demo-dotnet10 -n MyApp
```

以 admin 範本為例，產生的結構：

```
MyApp/
├── MyApp.slnx
├── MyApp.Server/           後端 Web API
├── myapp.client/           Vue 前端（目錄名自動轉小寫）
├── MyApp.Server.Tests/     xUnit 測試
├── DbScriptExporter/       DB script 匯出工具
└── db/                     schema.sql / seed.sql
```

命名空間、專案檔名、各個 port、`Jwt:SignKey` 都已自動替換為此專案專屬的值，不同專案之間不會互相衝突。

### 步驟 3：安裝前端套件

```bash
cd MyApp\myapp.client
pnpm install      # demo 範本請改用 npm install
cd ..
```

### 步驟 4：跑起來

```bash
dotnet run --project .\MyApp.Server\ --launch-profile https
```

SpaProxy 會自動把前端 dev server 一併拉起來，瀏覽器開啟 Scalar API 文件頁。

用 Visual Studio 的話直接開 `MyApp.slnx` 按 F5 即可，Server 已是預設 startup project。

admin 範本的登入帳密為 `admin` / `password`（in-memory 示範資料，**不需要資料庫**就能跑起來）。

### 步驟 5：產生專案後必須處理的事項

| 項目 | 說明 |
|------|------|
| 登入帳密 | `admin` / `password` 是寫死的示範帳號，正式環境務必更換 |
| `Jwt:SignKey` | 已自動隨機化為「專案名稱 + GUID」，正式環境建議改由 User Secrets、環境變數或 Key Vault 提供 |
| 資料庫（選用） | 預設為 in-memory 假資料，不接資料庫也能完整跑起來 |

**接上真實資料庫的步驟：**

1. 在 SQL Server 建立空白資料庫，依序執行 `db/schema.sql` 與 `db/seed.sql`
2. 修改 `appsettings.json` 的 `ConnectionStrings:Default` 指向該資料庫
3. 解開 `Features/Auth/UserRepository.cs`、`Features/Auth/GroupFeatureStore.cs`、`Features/Menu/MenuService.cs` 註解中現成的 Dapper SQL
4. seed 建立的帳號為 `admin` / `Admin@123`，同樣務必更換
5. `db/schema.sql` 中的 `Basic_Users.IdNumber` 標有 `TODO`，屬敏感個資欄位，請依專案需求決定去留

### 範本更新後

範本註冊的是資料夾路徑，因此 `git pull` 之後內容即自動生效，**一般情況不需要重新安裝**。只有這兩種情況需要處理：

```bash
# 資料夾搬移或改名
dotnet new uninstall <舊路徑>
dotnet new install <新路徑>

# 想強制刷新快取
dotnet new install .\vue-app-admin-dotnet10 --force
```

### 常見問題

**安裝時出現「下列範本使用相同的身分識別」警告**

代表同一個範本從兩個不同路徑各裝了一次（例如舊的測試目錄沒清掉）。`identity` 是範本在整台機器上的唯一鍵，重複時 `dotnet new` 只會啟用其中一個。執行不帶參數的 `dotnet new uninstall` 列出所有已安裝路徑，再把不要的那個解除安裝即可。

**`dotnet new list` 看不到範本**

先確認 `dotnet --version` 是否為 10.0 以上。兩個範本都有 SDK 版本約束，在舊版 SDK 下會被隱藏而不是報錯。

**`Failed to launch the SPA development server 'pnpm run dev'` / 系統找不到指定的檔案**

沒有安裝 pnpm。admin 範本的 SpaProxy 以 `pnpm run dev` 啟動前端，執行 `npm i -g pnpm` 後重跑即可。若已安裝 pnpm 仍失敗，確認 `<專案名稱>.client` 底下已有 `node_modules`（即步驟 3 的 `pnpm install` 已完成）。

---

## 範本清單

| 範本名稱 | 簡短名稱 | 語言 | 標記 |
|---------|---------|------|------|
| ASP.NET Core + Vue 3 (Admin, .NET 10) | `vue-app-admin-dotnet10` | [C#] | Web/SPA/Vue/ASP.NET Core |
| ASP.NET Core + Vue 3 (Demo, .NET 10) | `vue-app-demo-dotnet10` | [C#] | Web/SPA/Vue/ASP.NET Core |

短名稱與 .NET 8 版本不同，兩組範本可同時安裝、互不衝突；
產生專案時使用的 port 範圍也刻意錯開（.NET 8 版為 151xx–165xx，本版為 171xx–185xx）。

---

## 可用範本

### vue-app-admin-dotnet10

以 `vue-app-demo-dotnet10` 為基礎，加入後台管理系統完整骨架的全端專案範本。

**前端**

- Vue 3（Composition API）+ TypeScript
- Vite 8，`/api` proxy，HTTPS dev cert 自動產生
- Pinia（含 auth-store、user-info-store，含 loading / error state）
- Vue Router 5，含 beforeEach 登入守衛、404 catch-all、document.title 更新
- PrimeVue 4（Aura theme）+ Bootstrap 5 + Bootstrap Icons + FontAwesome 7
- Vee-Validate + Yup 表單驗證
- 集中式 axios instance（Bearer token 自動注入、ApiResponse<T> unwrap、401 自動登出）
- Feature-level API modules（`src/api/`）+ server 合約 TypeScript 型別（`src/types/api.ts`）
- `useTheme` composable — dark/light mode 手動切換，狀態存 localStorage，初始值 fallback 至 `prefers-color-scheme`
- Admin layout 以 CSS Grid 排版（sidebar 寬度由內容決定，`minmax(8rem, max-content)`），不寫死 px

**後端**

- ASP.NET Core Web API（.NET 10）
- JWT Bearer 認證（`AuthController`、`JwtService`、`AuthService`）
- Serilog（Console + rolling file，`.txt` 與 `.json`）
- Feature-based 垂直切片架構（`Features/<feature>/`）
- Request / Response 分層結構
- ExampleItems Controller + Service（hardcoded dummy data 示範分層）
- **內建 OpenAPI（`Microsoft.AspNetCore.OpenApi`）產生 OpenAPI 3.1 文件於 `/openapi/v1.json`，UI 使用 Scalar（`/scalar`）**
- **`Shared/OpenApi/BearerSecuritySchemeTransformer.cs` 以 `IOpenApiDocumentTransformer` 補上 JWT Bearer security scheme 與全域 security requirement**
- 統一 `ApiResponse<T>` 回應包裝格式；分頁端點另有 `ApiPagedResponse<T>`（含 `Total`，提供 `OkPaged` 工廠方法）
- ExampleItems 示範 server-side 分頁排序（`skip` / `top` / `sortField` / `sortOrder` query params，PrimeVue DataTable lazy 模式）

**資料庫**

- `db/schema.sql`：SQL Server schema，`Basic_*`（使用者、群組、選單、權限、登入與操作紀錄）與 `Para_*`（參數設定）共 11 張資料表，每個欄位皆有 `MS_Description`
- `db/seed.sql`：初始資料（admin 帳號、對齊前端路由的選單樹、Administrators 全權限群組）
- Auth / Menu / FeatureList 目前維持 in-memory dummy 實作；`UserRepository.cs`、`GroupFeatureStore.cs`、`MenuService.cs` 的註解已附上對應 `db/schema.sql` 的完整 Dapper SQL，要接真實資料庫時可直接解開使用

**測試**

- xUnit 測試專案（`VueAppAdmin.Server.Tests`，`net10.0`）
- NSubstitute mock 框架
- Coverlet 程式碼覆蓋率收集

**開發體驗**

- 登入後進入 MainLayout（Header + Sidebar 從 `router meta.showInSidebar` / `sidebarIcon` 自動衍生）
- Header 右上角 dark/light mode 切換按鈕（太陽 / 月亮 icon），PrimeVue 與 Bootstrap 同步切換
- 帳號 `admin` / 密碼 `password`（in-memory dummy 登入用；接上 `db/seed.sql` 後的 DB 帳號為 `admin` / `Admin@123`，兩者皆僅供開發使用，正式環境請務必更改）
- `appsettings.json` 的 `Jwt:SignKey` 於 `dotnet new` 產生專案時已自動置換為「專案名稱 + 隨機值」，不同專案彼此不同；正式上線建議改由 Secret 管理機制提供
- 產生專案後，終端機會顯示下一步指引（`npm install`；API 文件位置；以及選用的資料庫建置步驟：執行 `db/schema.sql` 與 `db/seed.sql`）

**前置需求**

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) >= 20.19.0

**安裝**

```bash
dotnet new install .\vue-app-admin-dotnet10
```

**建立新專案**

```bash
dotnet new vue-app-admin-dotnet10 -n MyApp
cd MyApp
dotnet run --project MyApp.Server
```

---

### vue-app-demo-dotnet10

以 Visual Studio 內建的 **Vue 和 ASP.NET Core** 專案類型為基礎，擴充額外工具鏈的全端專案範本。

**前端**

- Vue 3（Composition API）
- Vite 8，支援 HMR 熱更新
- TypeScript
- ESLint + oxlint

**後端**

- ASP.NET Core Web API（.NET 10）
- 內建 OpenAPI（`Microsoft.AspNetCore.OpenApi`）+ Scalar UI

**開發體驗**

- 一個 `dotnet run` 同時啟動前後端
- Vite 開發伺服器透過 SpaProxy 將 API 請求代理至 ASP.NET Core
- 透過 `dotnet dev-certs` 自動設定 HTTPS

**前置需求**

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) >= 20.19.0

**安裝**

```bash
dotnet new install .\vue-app-demo-dotnet10
```

**建立新專案**

```bash
dotnet new vue-app-demo-dotnet10 -n MyApp
cd MyApp
dotnet run --project MyApp.Server
```

---

## 從 .NET 8 版本移植的變更

| 項目 | .NET 8 版本 | 本版（.NET 10） |
|------|------------|----------------|
| `TargetFramework` | `net8.0`（測試專案為 `net9.0`） | `net10.0`（全部專案一致） |
| API 文件產生器 | `Swashbuckle.AspNetCore` 6.6.2 | `Microsoft.AspNetCore.OpenApi` 10.\*（OpenAPI 3.1） |
| API 文件 UI | Swagger UI（`/swagger`） | Scalar（`/scalar`），原始文件在 `/openapi/v1.json` |
| JWT security scheme | `AddSwaggerGen` 的 `AddSecurityDefinition` / `AddSecurityRequirement` | `IOpenApiDocumentTransformer`（`BearerSecuritySchemeTransformer`） |
| `launchSettings.json` | `"launchUrl": "swagger"` | `"launchUrl": "scalar"` |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | `8.*` | `10.*` |
| `Microsoft.AspNetCore.SpaProxy` | `8.*-*` | `10.*-*` |
| `Microsoft.Data.SqlClient` | 7.0.1 | 7.0.2 |
| `Microsoft.SqlServer.SqlManagementObjects` | 181.25.0 | 181.36.0 |
| 測試套件 | Test.Sdk 17.12 / xunit 2.9.2 / NSubstitute 5.3 / coverlet 6.0.2 | Test.Sdk 18.9 / xunit 2.9.3 / NSubstitute 6.2 / coverlet 10.0.1 |
| Admin `postActions` 指引文字 | 寫 `npm install`，與實際的 `pnpm run dev` 不一致 | 改為 `pnpm install`，並提示未安裝時先 `npm i -g pnpm` |
| Demo 範本方案檔 | `VueApp1.sln`（傳統格式） | `VueApp1.slnx`（XML 格式，與 admin 一致） |
| Demo `template.json` `guids` | 三個 GUID（且與 `.sln` 實際值不符，未生效） | 移除（`.slnx` 不含 GUID） |
| 範本短名稱 | `vue-app-admin-dotnet8`、`vue-app-demo` | `vue-app-admin-dotnet10`、`vue-app-demo-dotnet10` |
| Port 範圍 | 151xx–165xx | 171xx–185xx |
| SDK 約束 | 無 | `sdk-version: [10.0-*)` |

Vue / Vite / PrimeVue 等前端相依維持與來源 repo 相同版本，本次移植未變動。

**`Microsoft.OpenApi` v2 命名空間變更**

.NET 10 隨附的 `Microsoft.OpenApi` 已升到 v2，`OpenApiDocument`、`OpenApiSecurityScheme`
等型別從 `Microsoft.OpenApi.Models` 移至 `Microsoft.OpenApi`。撰寫 document transformer 時
要 `using Microsoft.OpenApi;`（不是 `Microsoft.OpenApi.Models`），
security requirement 的 key 也改用 `OpenApiSecuritySchemeReference`。

---

## 規格驅動開發流程（OpenSpec）

本 repo 沿用上游的 [OpenSpec](https://github.com/Fission-AI/OpenSpec) 工作流作為維護基礎。改動範本前先寫規格，實作完再歸檔，讓「範本現在到底該長怎樣」有單一可查證的來源。

**目錄**

| 路徑 | 用途 |
|------|------|
| `openspec/config.yaml` | 專案慣例（繁中撰寫規範、proposal / spec / tasks 的品質規則） |
| `openspec/specs/<capability>/spec.md` | **現行事實**：範本現在應該具備的行為，共 25 個 capability、127 條 requirement |
| `openspec/changes/<name>/` | 進行中的變更（proposal / design / tasks / spec deltas） |
| `openspec/changes/archive/` | 已完成並歸檔的變更 |
| `.claude/skills/openspec-*/` | 驅動流程的 Claude Code skills |

**capability 命名**

單層扁平 id，前綴標示適用範圍：

- `admin-*`：只適用於 `vue-app-admin-dotnet10`（如 `admin-backend-auth`、`admin-logging`）
- `demo-*`：只適用於 `vue-app-demo-dotnet10`
- 無前綴：兩個範本共通（`target-framework`、`solution-startup-order`）

> 上游使用巢狀目錄（`vue-app-admin-dotnet8/<cap>/`），但 openspec CLI 只掃單層，導致 `openspec list --specs` 顯示 `requirements 0`、`openspec validate` 全數失敗。本 repo 改為扁平 id 並補上 `## Purpose` / `## Requirements` 標頭後才真正可驗證。

**日常操作**

```bash
npm install -g openspec        # 安裝 CLI（本 repo 以 v1.2.0 驗證）

openspec list --specs          # 列出所有 capability 與 requirement 數
openspec show <capability>     # 檢視單一 capability
openspec validate --all        # 驗證全部規格結構（送出變更前必跑）

openspec list                  # 列出進行中的變更
openspec status --change <name>
openspec archive <name>        # 實作完成後歸檔，並把 delta 併入 specs/
```

在 Claude Code 中則直接用對應的 skill：`openspec-explore`（探索現況）、`openspec-propose`（提變更）、`openspec-apply-change`（實作）、`openspec-archive-change`（歸檔）。

**歷史**

`openspec/changes/archive/` 目前只有 `2026-09-03-port-templates-to-dotnet10` 一筆，即本次由 .NET 8 移植的完整紀錄（含踩到的坑與取捨）。移植前的沿革留在[上游 repo](https://github.com/littlehorseboy/dotnet-new-templates) 的 `openspec/changes/archive/`——那 15 筆全部指向 `vue-app-admin-dotnet8`、Swagger 與 `net8.0`，整批複製過來只會與本 repo 現況互相矛盾，因此未搬移。

---

## 維護筆記

### `Microsoft.VisualStudio.JavaScript.Sdk` 版本

各範本的 `*.client.esproj` 中有一行：

```xml
<Project Sdk="Microsoft.VisualStudio.JavaScript.Sdk/1.0.2752196">
```

這個版本號跟著 Visual Studio 安裝器走，**不會自動更新**。建議在 VS 升級後手動確認：

1. 在 VS 建立任意 TypeScript 或 JavaScript 專案
2. 開啟產生的 `.esproj`，比對版本號
3. 若有新版本，更新此 repo 中所有 `.esproj` 的版本號

或直接查 NuGet：搜尋 `Microsoft.VisualStudio.JavaScript.Sdk` 取得最新版本。

舊版本向後相容，不急著更新，定期（每次 VS 大版升級後）確認即可。
