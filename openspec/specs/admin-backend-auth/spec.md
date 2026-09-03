## Purpose

規範後端 JWT 認證的組成方式：設定注入、token 簽發與驗證、受保護端點的預設行為。

## Requirements

### Requirement: JWT 認證設定透過擴充方法注入
`Shared/Jwt/JwtExtensions.cs` SHALL 提供 `AddJwtAuthentication(IConfiguration)` 擴充方法，將 JWT Bearer 驗證設定注入 DI 容器，`Program.cs` 只需一行呼叫。

#### Scenario: JWT 設定正確載入
- **WHEN** `Program.cs` 呼叫 `builder.Services.AddJwtAuthentication(builder.Configuration)`
- **THEN** JWT Bearer 驗證以 `appsettings.json` 的 `Jwt` section 為設定來源，且 `ValidateLifetime`、`ValidateIssuerSigningKey` 均為 `true`

### Requirement: JWT 設定以強型別 Options 管理
`JwtOptions.cs` DTO SHALL 包含 `Issuer`（string）、`SignKey`（string）欄位，並以 `[Required]` 標注，`ValidateDataAnnotations().ValidateOnStart()` 確保啟動期即驗證。

#### Scenario: 缺少 JWT 設定時啟動失敗
- **WHEN** `appsettings.json` 中 `Jwt:SignKey` 為空或缺漏
- **THEN** 應用程式在啟動時拋出例外，而非執行期才失敗

### Requirement: 登入端點驗證帳號密碼並回傳 JWT
`AuthController` SHALL 提供 `POST /api/Auth/Login` 端點，接受 `LoginRequest`（`Username`、`Password`），驗證成功後回傳含 JWT token 的 `LoginResponse`。

#### Scenario: 有效帳密登入成功
- **WHEN** 使用者 POST 有效的 `username` 與 `password`
- **THEN** 回應 HTTP 200，body 包含 `token` 字串

#### Scenario: 無效帳密登入失敗
- **WHEN** 使用者 POST 無效的帳號或密碼
- **THEN** 回應 HTTP 401

### Requirement: 取得目前使用者資訊端點
`AuthController` SHALL 提供 `GET /api/Auth/Me` 端點（需 JWT），回傳目前登入使用者的基本資訊（`username`、`displayName`）。

#### Scenario: 有效 token 取得使用者資訊
- **WHEN** 已登入使用者以有效 JWT Bearer token 呼叫 `GET /api/Auth/Me`
- **THEN** 回應 HTTP 200 含使用者資訊

#### Scenario: 無 token 存取被拒
- **WHEN** 未攜帶 token 呼叫 `GET /api/Auth/Me`
- **THEN** 回應 HTTP 401

### Requirement: AuthService 提供帳密驗證與使用者查詢
`AuthService` SHALL 提供 `ValidateCredentials(username, password)` 方法，範本版本使用 hardcoded 帳號（`admin` / `password`）供驗證。

#### Scenario: 帳密驗證（範本 hardcoded）
- **WHEN** 呼叫 `ValidateCredentials("admin", "password")`
- **THEN** 回傳 `true`

### Requirement: JwtService 負責 token 產生與驗證
`JwtService` SHALL 提供 `GenerateToken(username, displayName)` 方法，依 `JwtOptions` 產生帶有 claims 的 JWT token。

#### Scenario: 產生有效 JWT
- **WHEN** 呼叫 `GenerateToken("admin", "Administrator")`
- **THEN** 回傳可被 `JwtExtensions` 設定的驗證參數驗證通過的 JWT 字串

---

### Requirement: JWT Token 到期時間可透過設定檔控制

系統 SHALL 從 `appsettings.json` 的 `Jwt:TokenExpirationHours` 讀取 Token 到期時數，而非 hardcode。`JwtOptions` SHALL 提供 `TokenExpirationHours` 屬性，預設值為 `8`，確保設定檔缺少該欄位時行為不變。

#### Scenario: 設定檔包含 TokenExpirationHours

- **WHEN** `appsettings.json` 的 `Jwt:TokenExpirationHours` 設為 `24`
- **THEN** 登入後取得的 JWT Token 到期時間距離簽發時間為 24 小時

#### Scenario: 設定檔未包含 TokenExpirationHours

- **WHEN** `appsettings.json` 的 `Jwt` 區段未包含 `TokenExpirationHours`
- **THEN** Token 到期時間預設為 8 小時，行為與修改前相同
