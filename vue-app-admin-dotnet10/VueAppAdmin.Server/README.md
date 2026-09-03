# VueAppAdmin.Server

ASP.NET Core 8 WebAPI，採用 by-feature 資料夾結構。

---

## 專案結構

```
VueAppAdmin.Server/
├── Features/
│   ├── Auth/
│   │   ├── Helpers/          # PasswordHasherHelper
│   │   ├── Requests/         # LoginRequest（DataAnnotations 驗證）
│   │   ├── Responses/        # LoginResponse、MeResponse
│   │   ├── AuthController.cs
│   │   ├── AuthExtensions.cs # AddAuthFeature() — DI 自我註冊
│   │   ├── AuthService.cs    # 實作 IAuthService
│   │   ├── IAuthService.cs
│   │   ├── IUserRepository.cs
│   │   └── UserRepository.cs # Dapper，SQL 集中於此
│   ├── ExampleItems/         # 示範用 Feature（in-memory，無 DB）
│   │   ├── Requests/
│   │   ├── Responses/
│   │   ├── ExampleItemsController.cs
│   │   ├── ExampleItemsExtensions.cs
│   │   ├── ExampleItemsService.cs
│   │   └── IExampleItemsService.cs
│   ├── ExampleCategories/    # 示範用類別 Feature（in-memory）
│   │   └── Responses/
│   ├── FeatureList/          # 功能識別字清單 Feature
│   │   └── Responses/
│   └── Menu/                 # 選單（依使用者 features 過濾）
├── Shared/
│   ├── ApiResponse.cs        # 統一回傳型別 ApiResponse<T>
│   ├── Database/             # IDbConnection Scoped 註冊
│   ├── Jwt/                  # JWT 設定、IJwtService、JwtExtensions
│   ├── Logging/              # SerilogHelper、ApiLogFilter、LogMaskAttribute
│   ├── Middleware/           # ExceptionHandlingMiddleware
│   └── OpenApi/              # BearerSecuritySchemeTransformer（JWT security scheme）
└── Program.cs
```

### 規則
- 每個業務功能放在 `Features/<FeatureName>/`，包含該功能所有相關檔案
- 跨 Feature 共用的基礎設施放在 `Shared/<TopicName>/`
- 每個 Feature 提供 `Add<Name>Feature()` 擴充方法，在 `Program.cs` 呼叫

---

## 設定

### `appsettings.json`

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.AspNetCore": "Warning"
      }
    }
  },
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=VueAppAdmin;..."
  },
  "Jwt": {
    "Issuer": "VueAppAdmin",
    "SignKey": "VueAppAdmin-REPLACE_WITH_A_STRONG_SECRET_KEY_AT_LEAST_32_CHARS",
    "TokenExpirationHours": 8
  },
  "Logging": {
    "RetentionDays": 365
  }
}
```

> **重要**：`Jwt:SignKey` 在範本目錄本身是固定佔位字串；透過 `dotnet new` 產生專案時，`.template.config/template.json` 的 `JwtSecret` symbol 會自動將其替換為「專案名稱 + 隨機值」，每個產出專案的金鑰皆不相同，無需手動更改。若直接使用本範本目錄開發（未經 `dotnet new` 產生），請自行替換為至少 32 字元的強密鑰。

---

## 資料庫建置（選用）

`Auth` / `Menu` / `FeatureList` 三個 Feature 目前維持 in-memory dummy 實作，**不需要資料庫即可執行**，預設登入帳密為 `admin` / `password`（僅供展示）。

`db/schema.sql`、`db/seed.sql`（位於方案根目錄，`dotnet new` 產生專案時會一併產出）提供 SQL Server schema 與初始資料，供未來要接上真實資料庫時使用：

```bash
# 於 SQL Server 建立空白資料庫後，依序執行：
# 1. db/schema.sql — 建立 11 張資料表（Basic_*、Para_*），每個欄位皆有 MS_Description
# 2. db/seed.sql   — 建立 admin 帳號、選單樹、Administrators 全權限群組
```

- `db/seed.sql` 的帳號為 `admin` / `Admin@123`（內含此密碼實際產生並驗證通過的 BCrypt 雜湊），**僅在接上真實資料庫後生效**，與上方 in-memory 預設的 `admin` / `password` 是兩組不同的帳密。**兩組正式環境上線前都務必更改。**
- `Basic_Users.IdNumber` 欄位標記為 `TODO`：是否保留此敏感個資欄位，請依專案需求自行決定去留。
- `Basic_Api_Log` / `Basic_Api_Change_Log` 皆有 `RequestId` 欄位，對應 `HttpContext.TraceIdentifier`，可串接同一次 API 呼叫的完整記錄與其觸發的資料異動；`Basic_Api_Log` 另新增 `Method`/`Path`/`StatusCode`/`ElapsedMs` 欄位對應 `ApiLogFilter` 的記錄內容，`Datas` 欄位加上 `CHECK(ISJSON(Datas)=1)` 限制，寫入時須為合法 JSON 字串。
- 要接上真實資料庫時，`UserRepository.cs`、`GroupFeatureStore.cs`、`MenuService.cs` 的註解中已附上對應 `db/schema.sql` 資料表的完整 Dapper SQL，可直接解開並依 `Shared/Database/DatabaseExtensions.cs` 的既有 `IDbConnection` 注入方式串接。

---

## 啟動

```bash
# 在方案根目錄（含 .slnx）時，需指定專案路徑
dotnet run --project .\VueAppAdmin.Server\ --launch-profile https

# 已在 VueAppAdmin.Server/ 目錄內時可直接執行
dotnet run --launch-profile https
```

API 文件（Development 環境）：

- Scalar UI：`https://localhost:7173/scalar`
- OpenAPI 3.1 文件：`https://localhost:7173/openapi/v1.json`

由 .NET 10 內建的 `Microsoft.AspNetCore.OpenApi` 產生，JWT Bearer 的 security scheme
由 `Shared/OpenApi/BearerSecuritySchemeTransformer.cs` 補上，可在 Scalar 右上角 Authorize 填入 token。

---

## API 端點

所有端點需 JWT Bearer Token，除了 `POST /api/auth/login`。

| Method | 路徑 | 說明 |
|--------|------|------|
| POST | `/api/Auth/Login` | 登入，回傳 Token |
| GET | `/api/Auth/Me` | 取得目前登入者資訊（username、groups、features） |
| POST | `/api/ExampleItems/Search` | 分頁搜尋範例清單（含篩選、日期區間查詢、全欄位排序） |
| GET | `/api/ExampleItems/{id}` | 取得單筆範例 |
| POST | `/api/ExampleCategories` | 取得所有類別清單 |
| GET | `/api/Features` | 取得系統所有功能識別字清單 |
| POST | `/api/Menu/Items` | 取得依使用者功能過濾後的選單樹 |

### 回傳格式 `ApiResponse<T>`

```json
// 成功（單筆）
{ "success": true, "message": null, "result": { ... }, "results": null }

// 成功（集合）
{ "success": true, "message": null, "result": null, "results": [ ... ] }

// 失敗
{ "success": false, "message": "錯誤說明", "result": null, "results": null }
```

---

## Logging

| 檔案 | 內容 |
|------|------|
| `logs/log-system-<date>.txt` | 系統層 log，含 framework 訊息（DI `ILogger<T>`） |
| `logs/log-<date>.txt` | 靜態 `Log.*`，用於啟動與崩潰記錄 |
| `logs/ApiLogFilter/ApiLogFilter-<date>.txt` | API 請求 / 回應記錄（獨立檔案） |

### Log Level 設定

Log level 透過 `appsettings.json` 的 `Serilog:MinimumLevel` 控制：

```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",   // 全域預設等級
    "Override": {
      "Microsoft.AspNetCore": "Warning"  // 指定 namespace 覆寫等級
    }
  }
}
```

`appsettings.Development.json` 可將 `Default` 設為 `Debug`，開發時取得更細的 log 輸出。

可用等級由低到高：`Verbose` → `Debug` → `Information` → `Warning` → `Error` → `Fatal`

> **注意**：應用程式啟動初期（DI 容器建立之前）使用的 bootstrap logger（`SerilogHelper.Initialize()`）固定為 `Information` 等級，不受 appsettings 設定影響。這是 Serilog 兩階段初始化的已知限制。

### API 全域 Log（ApiLogFilter）

所有 API 端點自動記錄，涵蓋三個案例：

| 案例 | Level | 格式 |
|------|-------|------|
| 正常流程（2xx、4xx 業務錯誤） | INF | `[API] {Method} {Path} \| user:{User} \| {StatusCode} \| req:{...} \| res:{...} \| {N}ms \| reqId:{RequestId}` |
| 401 授權短路 | WRN | 同上，`req` 與 `res` 為 `null` |
| 400 驗證失敗 | WRN | 同上，`res` 顯示驗證錯誤欄位 |

**敏感欄位遮罩**：在 Request 類別的屬性加上 `[LogMask]`，該欄位在 log 中顯示為 `***`。

```csharp
[LogMask]
public string Password { get; set; }
```

**分頁回應精簡**：`ApiPagedResponse<T>` 的 `res` 只記錄 `{ success, total, count }`，不含完整 items 清單。

**請求關聯（reqId）**：每筆 log 皆附上 `reqId:{RequestId}`，對應 `HttpContext.TraceIdentifier`。`ApiLogFilter`（正常流程）與 `ExceptionHandlingMiddleware`（未攔截例外）共用同一組 `TraceIdentifier`，可用 reqId 串接同一次 API 呼叫的完整記錄，無論該次呼叫是否拋出例外。

> TODO：未來 `ApiLogs` 存表功能實作後，可在 `ApiResponse<T>.ToLogSummary()` 切換為精簡格式，完整 payload 改由資料庫保存；`db/schema.sql` 的 `Basic_Api_Log` 已備妥 `RequestId`/`Method`/`Path`/`StatusCode`/`ElapsedMs` 欄位可直接對應。

### 為特定服務建立獨立 log 資料夾

```csharp
// 寫入 logs/SmsService/SmsService-<date>.txt
private static readonly Serilog.ILogger _logger = SerilogHelper.GetLogger<SmsService>();

_logger.Information("Sending SMS to {Phone}", phone);
```

適合場景：排程任務、外部整合（SMS/Email/Push）、稽核 log。
一般 Service / Controller 請使用 DI 注入的 `ILogger<T>`。

---

## 新增 Feature

以新增 `Products` Feature 為例：

**1. 建立資料夾結構**
```
Features/Products/
├── Requests/     CreateProductRequest.cs
├── Responses/    ProductResponse.cs
├── IProductsService.cs
├── ProductsService.cs
├── IProductRepository.cs
├── ProductRepository.cs
├── ProductsController.cs
└── ProductsExtensions.cs
```

> **Repository 何時出現**：只有當 Service 需要真正查詢外部資料來源（資料庫、外部 API 等）時才需要 `I<Xxx>Repository` / `<Xxx>Repository.cs`，把 SQL / 資料存取邏輯跟 Service 的商業邏輯分開，Service 改為注入該介面。若 Service 資料是寫死在記憶體的示範資料（如 `ExampleItems`、`ExampleCategories`、`FeatureList`、`Menu`），沒有東西好抽離，就不需要 Repository。可參考 `Auth/IUserRepository.cs`、`Auth/UserRepository.cs`（Dapper 實作）作為真接資料庫時的範例。

**2. `ProductsExtensions.cs`**
```csharp
public static class ProductsExtensions
{
    public static IServiceCollection AddProductsFeature(this IServiceCollection services)
    {
        services.AddScoped<IProductsService, ProductsService>();
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}
```

**3. `Program.cs` 加入一行**
```csharp
builder.Services.AddProductsFeature();
```

---

## 測試

```bash
dotnet test
```

測試專案：`VueAppAdmin.Server.Tests`（xUnit + NSubstitute）

測試結構對應 `Features/` 資料夾，Service 的 Repository 相依以 NSubstitute mock 替換。
