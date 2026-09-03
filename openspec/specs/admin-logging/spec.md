## Purpose

規範 Serilog 的設定、輸出目的地、保留天數、API log filter 與敏感欄位遮罩行為。

## Requirements

### Requirement: Serilog 基礎設定
應用程式 SHALL 使用 Serilog 作為 logging 框架，在 `Shared/Logging/SerilogHelper.cs` 中集中管理初始化設定。

#### Scenario: Console 與檔案 sink
- **WHEN** 應用程式啟動
- **THEN** Serilog SHALL 同時輸出至 Console 與每日 rolling txt 檔案（`logs/log-.txt`），保留天數 SHALL 為 365 天，不得使用 JSON sink

#### Scenario: 每日 rolling 檔名
- **WHEN** 跨越午夜
- **THEN** 系統 SHALL 自動建立新的 log 檔案，舊檔案保留，超過 365 個檔案時 SHALL 刪除最舊的

---

### Requirement: HTTP Request Logging
每一次 HTTP request/response SHALL 被自動記錄，包含 HTTP method、路徑、狀態碼、耗時。

#### Scenario: 正常請求記錄
- **WHEN** 任何 HTTP 請求進入並完成
- **THEN** Serilog SHALL 記錄一行 Information 等級的 log，包含 method、path、status code、duration（毫秒）

#### Scenario: `UseSerilogRequestLogging` 位置
- **WHEN** 設定 middleware pipeline
- **THEN** `app.UseSerilogRequestLogging()` SHALL 在 `UseAuthentication()` 之前加入 pipeline

---

### Requirement: Service 層結構化 Logging
Service 類別 SHALL 透過建構子注入 `ILogger<T>` 進行 logging，不得使用靜態 `Log.*` 方法。

#### Scenario: ILogger 注入
- **WHEN** 建立任何 Service 類別
- **THEN** SHALL 透過 primary constructor 注入 `ILogger<T>`（T 為該 Service 型別）

#### Scenario: 結構化 log 參數
- **WHEN** 記錄含有變數的 log
- **THEN** SHALL 使用具名佔位符（`{Username}`），不得使用字串插值（`$"{username}"`）

---

### Requirement: 驗證失敗 Logging
DataAnnotations 驗證失敗時，系統 SHALL 記錄 Warning 等級 log，包含路徑、HTTP 狀態碼與驗證錯誤詳情。

記錄行為 SHALL 由 `ApiLogFilter` 的 `IAlwaysRunResultFilter` 路徑統一處理，`InvalidModelStateResponseFactory` 中的 `LogWarning` 呼叫 SHALL 移除，以維持單一 log 來源。

#### Scenario: 驗證失敗記錄格式
- **WHEN** request 的 ModelState 驗證失敗（400）
- **THEN** `ApiLogFilter` SHALL 記錄 `LogWarning`，格式為 `[API] {Method} {Path} | user:{User} | {StatusCode} | req:{@Request} | res:{@Response} | {ElapsedMs}ms`，其中 `res` 包含 validation errors

#### Scenario: 不重複記錄
- **WHEN** 驗證失敗發生
- **THEN** 整個請求生命週期中 SHALL 只出現一筆驗證失敗 log，`InvalidModelStateResponseFactory` 不得再單獨呼叫 `LogWarning`

---

### Requirement: 全域 Exception 集中 Logging
所有未被捕捉的 exception SHALL 由 `ExceptionHandlingMiddleware` 攔截並記錄，位於 `Shared/Middleware/ExceptionHandlingMiddleware.cs`。

記錄內容 SHALL 包含 `HttpContext.TraceIdentifier` 作為 correlation id（`reqId`），且欄位名稱須與 `ApiLogFilter` 正常路徑一致，使兩條路徑寫出的 log 可透過同一個值互相對應查詢。

#### Scenario: Unhandled exception 記錄
- **WHEN** 任何未捕捉的 exception 發生
- **THEN** middleware SHALL 記錄 `LogError` 包含 exception 物件、HTTP method、路徑、correlation id，並回傳 HTTP 500

#### Scenario: Exception detail 不洩漏
- **WHEN** 回傳 500 錯誤給前端
- **THEN** response body SHALL 只包含通用錯誤訊息，不得包含 exception message 或 stack trace

#### Scenario: Middleware 在 pipeline 的位置
- **WHEN** 設定 middleware pipeline
- **THEN** `ExceptionHandlingMiddleware` SHALL 是 pipeline 中最外層的 middleware（第一個 `app.Use*` 呼叫）

---

### Requirement: 全域 API Log Filter
系統 SHALL 透過全域 `ApiLogFilter`（`Shared/Logging/ApiLogFilter.cs`）統一記錄所有 API 端點的 request 與 response，涵蓋正常流程、401 授權短路、400 驗證失敗三個案例。

`ApiLogFilter` SHALL 實作 `IActionFilter` 與 `IAlwaysRunResultFilter`，並在 `Program.cs` 以 `options.Filters.Add()` 全域套用。

每筆 log SHALL 包含 `HttpContext.TraceIdentifier` 作為 correlation id（`reqId`），使正常路徑與 `ExceptionHandlingMiddleware` 的例外路徑可透過同一個值互相對應。

#### Scenario: 正常流程記錄
- **WHEN** 一個通過授權與驗證的 API 請求完成執行
- **THEN** `ApiLogFilter` SHALL 以 `LogInformation` 記錄一筆 log，包含 HTTP method、path、user identity、status code、masked request payload、response 內容、elapsed time（毫秒）、correlation id，格式為 `[API] {Method} {Path} | user:{User} | {StatusCode} | req:{@Request} | res:{@Response} | {ElapsedMs}ms | reqId:{RequestId}`

#### Scenario: 401 授權短路記錄
- **WHEN** 請求因 `[Authorize]` 全域 filter 未通過授權而短路（HTTP 401）
- **THEN** `ApiLogFilter` SHALL 以 `LogWarning` 記錄一筆 log，包含 method、path、user（空值以 `-` 表示）、status code、correlation id；request body 因 model binding 尚未執行，`req` 欄位記錄為 `null`

#### Scenario: 400 驗證失敗記錄
- **WHEN** request 的 DataAnnotations 驗證失敗（HTTP 400）
- **THEN** `ApiLogFilter` SHALL 以 `LogWarning` 記錄一筆 log，包含 method、path、user、status code、masked request payload、correlation id；validation errors 以 `{@Response}` 欄位呈現

#### Scenario: Filter 執行時機
- **WHEN** 設定 MVC filter pipeline
- **THEN** `ApiLogFilter` SHALL 透過 `Program.cs` 的 `options.Filters.Add(new ApiLogFilter(...))` 全域套用，不需在各 Controller/Action 個別標記

---

### Requirement: 敏感欄位遮罩（LogMask）
系統 SHALL 提供 `[LogMask]` attribute（`Shared/Logging/LogMaskAttribute.cs`），用於標記 Request 物件中的敏感欄位。`ApiLogFilter` 序列化 request 之前，SHALL 以 reflection 將所有標記欄位的值替換為 `"***"`。

#### Scenario: Password 欄位遮罩
- **WHEN** `LoginRequest.Password` 標記有 `[LogMask]`，且 Login API 被呼叫
- **THEN** log 中的 `req` 欄位 SHALL 顯示 `"password":"***"`，不得出現原始密碼值

#### Scenario: 未標記欄位不受影響
- **WHEN** Request 物件的欄位未標記 `[LogMask]`
- **THEN** 該欄位值 SHALL 原樣序列化進入 log

---

### Requirement: ApiResponse Log 摘要
`ApiResponse<T>` SHALL 提供 `ToLogSummary()` 方法，供 `ApiLogFilter` 呼叫以取得 response 的 log 表示。

`ApiPagedResponse<T>` SHALL 覆寫 `ToLogSummary()`，回傳包含 `Success`、`Total`、`Count`（items 筆數）的精簡物件，避免分頁資料全量寫入 log。

#### Scenario: 一般 ApiResponse log 內容
- **WHEN** `ApiLogFilter` 記錄正常流程的 response
- **THEN** log 的 `res` 欄位 SHALL 包含完整 `ApiResponse` 序列化內容（`Success`、`Message`、`Result`/`Results`）

#### Scenario: ApiPagedResponse log 內容
- **WHEN** `ApiLogFilter` 記錄分頁查詢的 response
- **THEN** log 的 `res` 欄位 SHALL 只包含 `{ success, total, count }`，不含完整 items 清單

#### Scenario: 預留存表擴充點（TODO）
- **WHEN** 未來「API Log 存表」功能實作
- **THEN** `ToLogSummary()` SHALL 可改為精簡格式，完整 payload 由 `ApiLogs` 資料庫表保存；目前以 TODO 註解標記於 `ToLogSummary()` 實作處

---

### Requirement: 登入紀錄存表（TODO 預留）
系統 SHALL 預留登入紀錄寫入資料庫的擴充點，本次不實作。

#### Scenario: TODO 標記位置
- **WHEN** `AuthService.ValidateCredentials` 完成登入驗證
- **THEN** 程式碼 SHALL 包含 TODO 註解，說明未來應將登入嘗試（帳號、結果、IP、時間）寫入 `LoginLogs` 資料表，供稽核查閱

---

### Requirement: Serilog MinimumLevel 可透過 appsettings.json 設定

系統 SHALL 使用 `Serilog.Settings.Configuration` 套件，讓 Serilog 的 MinimumLevel 從 `appsettings.json` 的 `Serilog:MinimumLevel` 區段讀取，取代原先 hardcode 的 `.MinimumLevel.Information()`。

#### Scenario: 正式環境 log level 讀取

- **WHEN** 應用程式在正式環境啟動
- **THEN** Serilog system logger SHALL 套用 `appsettings.json` 中 `Serilog:MinimumLevel:Default` 的設定值（預設為 `Information`）

#### Scenario: 開發環境 log level 讀取

- **WHEN** 應用程式在開發環境（`ASPNETCORE_ENVIRONMENT=Development`）啟動
- **THEN** Serilog system logger SHALL 套用 `appsettings.Development.json` 中 `Serilog:MinimumLevel:Default` 的設定值（預設為 `Debug`）

#### Scenario: Override 設定生效

- **WHEN** `appsettings.json` 的 `Serilog:MinimumLevel:Override` 包含 `Microsoft.AspNetCore: Warning`
- **THEN** 來自 `Microsoft.AspNetCore` namespace 的 log SHALL 僅記錄 `Warning` 以上等級，`Information` 以下丟棄

#### Scenario: bootstrap logger 不受 appsettings 控制

- **WHEN** 應用程式啟動，`IConfiguration` 尚未建立
- **THEN** `SerilogHelper.Initialize()` 的 bootstrap logger SHALL 固定使用 `Information` 等級，不讀取 appsettings.json

---

### Requirement: Serilog Log 保留天數可透過設定檔控制

系統 SHALL 從 `appsettings.json` 的 `Logging:RetentionDays` 讀取 Log 檔案保留天數，而非 hardcode。若設定檔未包含該值，SHALL fallback 至預設值 `365`，確保行為不變。

#### Scenario: 設定檔包含 RetentionDays

- **WHEN** `appsettings.json` 的 `Logging:RetentionDays` 設為 `30`
- **THEN** Serilog 每日 rolling log 檔案最多保留 30 天，超過自動刪除

#### Scenario: 設定檔未包含 RetentionDays

- **WHEN** `appsettings.json` 的 `Logging` 區段未包含 `RetentionDays`
- **THEN** Log 保留天數預設為 365 天，行為與修改前相同

---

### Requirement: 請求關聯 id 需貫穿正常與例外兩種結束路徑
系統 SHALL 對每一個 HTTP request 使用 `HttpContext.TraceIdentifier` 作為唯一的 correlation id，且無論該 request 是正常完成（由 `ApiLogFilter` 寫出 log）或因未處理例外而終止（由 `ExceptionHandlingMiddleware` 寫出 log），輸出的 log 訊息中 MUST 包含相同的 correlation id 值，且此值 SHALL 與資料庫中 `Basic_Api_Log.RequestId` 一致，可互相對應查詢。

#### Scenario: 同一次呼叫在文字 log 與資料庫紀錄中可互相對應
- **WHEN** 需要查證某一次 API 呼叫在 Serilog 文字 log 中的完整內容
- **THEN** 可使用該次呼叫在 `Basic_Api_Log.RequestId` 中的值，於 Serilog 文字 log 內查得對應該 correlation id 的紀錄，反之亦然

---

### Requirement: Basic_Api_Log 需具備可直接查詢的關鍵欄位
`Basic_Api_Log` 資料表 SHALL 提供獨立欄位以記錄 `RequestId`、`Method`、`Path`、`StatusCode`、`ElapsedMs`，使查詢時不需解析 `Datas` 欄位內的 JSON 內容。

#### Scenario: 依 StatusCode 篩選錯誤請求
- **WHEN** 需要找出所有回應狀態碼為 500 的 API 呼叫紀錄
- **THEN** 可直接對 `Basic_Api_Log.StatusCode` 欄位執行 SQL 篩選，不需解析 `Datas`

#### Scenario: 依 ElapsedMs 排序找出耗時最長的請求
- **WHEN** 需要找出耗時最長的 API 呼叫紀錄
- **THEN** 可直接對 `Basic_Api_Log.ElapsedMs` 欄位執行 SQL 排序

#### Scenario: ElapsedMs 僅反映整支 API 呼叫耗時
- **WHEN** 查詢 `Basic_Api_Log.ElapsedMs` 的值
- **THEN** 該值 SHALL 代表整支 API 呼叫（從 action 開始執行到回應完成）的總耗時，而非單一 SQL 查詢的執行時間

---

### Requirement: Basic_Api_Log.Datas 僅能存放合法 JSON
`Basic_Api_Log.Datas` 欄位 SHALL 透過資料庫層級的 CHECK constraint 強制其內容必為合法 JSON 或 NULL。

#### Scenario: 寫入合法 JSON 成功
- **WHEN** 對 `Basic_Api_Log.Datas` 寫入一段合法的 JSON 字串
- **THEN** 該筆 INSERT/UPDATE 成功執行

#### Scenario: 寫入非 JSON 字串被資料庫拒絕
- **WHEN** 對 `Basic_Api_Log.Datas` 寫入一段非合法 JSON 格式的字串
- **THEN** 該筆 INSERT/UPDATE SHALL 被資料庫的 CHECK constraint 拒絕並回傳錯誤

---

### Requirement: Basic_Api_Change_Log 需能串回觸發異動的 API 呼叫
`Basic_Api_Change_Log` 資料表 SHALL 提供 `RequestId` 欄位，記錄與該筆資料異動對應的 `Basic_Api_Log.RequestId` 相同的 correlation id 值。

#### Scenario: 透過 RequestId 找出造成資料異動的原始 API 呼叫
- **WHEN** 在 `Basic_Api_Change_Log` 中發現一筆資料異動紀錄
- **THEN** 可使用其 `RequestId` 值，在 `Basic_Api_Log` 中查得觸發此次異動的 API 呼叫詳細內容（`Method`、`Path`、`StatusCode` 等）

---

### Requirement: ModulesNames 與 Path 分別記錄 code 視角與 HTTP 視角
`Basic_Api_Log.ModulesNames` 欄位 SHALL 記錄實際處理該請求的類別與方法（格式：`Namespace.Class/Method`），`Path` 欄位 SHALL 記錄使用者實際呼叫的 HTTP 請求路徑，兩者並存且不互相取代。

#### Scenario: 依程式碼位置查詢處理某請求的類別與方法
- **WHEN** 需要找出是哪一個類別的哪一個方法處理了某筆請求
- **THEN** 可查詢 `Basic_Api_Log.ModulesNames`，取得格式為 `Namespace.Class/Method` 的值

#### Scenario: 依 HTTP 路徑查詢實際被呼叫的網址
- **WHEN** 需要找出使用者實際呼叫的 HTTP 路徑
- **THEN** 可查詢 `Basic_Api_Log.Path`，其值不受 `ModulesNames` 是否變動影響
