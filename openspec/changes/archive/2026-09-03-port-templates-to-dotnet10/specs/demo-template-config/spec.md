## MODIFIED Requirements

### Requirement: 範本可成功安裝
系統 SHALL 允許使用者透過 `dotnet new install .\vue-app-demo-dotnet10` 將此範本安裝為本機範本。`identity` SHALL 為 `VueAppDemoDotnet10.CSharp`、`shortName` SHALL 為 `vue-app-demo-dotnet10`。

> 原規格的安裝路徑為 `.\VueApp1`，與上游實際目錄名 `vue-app-demo` 早已不符。

#### Scenario: 安裝成功
- **WHEN** 使用者在 repo 根目錄執行 `dotnet new install .\vue-app-demo-dotnet10`
- **THEN** 指令回傳成功，`dotnet new list` 中出現短名稱 `vue-app-demo-dotnet10`

#### Scenario: 與 .NET 8 版範本並存
- **WHEN** 使用者同時安裝 .NET 8 版的 `vue-app-demo` 與本範本
- **THEN** 兩者皆出現於 `dotnet new list`，short name 與 identity 皆不重複

---

### Requirement: 專案名稱自動替換
系統 SHALL 將所有出現 `VueApp1` 的檔名、資料夾名稱與檔案內容，替換為使用者指定的 `-n` 名稱。方案檔名為 `MyApp.slnx`（原為 `MyApp.sln`）。

#### Scenario: 方案名稱替換
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** 產生的方案檔名稱為 `MyApp.slnx`，內容中不含 `VueApp1`

#### Scenario: 命名空間替換
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** C# 檔案中的命名空間為 `MyApp.Server`，不含 `VueApp1`

#### Scenario: 小寫資料夾名稱替換
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** 前端目錄名稱為 `myapp.client`（全小寫），不含 `vueapp1`

## ADDED Requirements

### Requirement: 範本限定 .NET 10 以上的 SDK
`template.json` SHALL 宣告 `constraints`，以 `sdk-version` 約束 `[10.0-*)`。

#### Scenario: 僅有舊版 SDK 的環境
- **WHEN** 使用者在僅安裝 .NET 8/9 SDK 的機器執行 `dotnet new list`
- **THEN** `vue-app-demo-dotnet10` 不出現在可用範本清單中

---

### Requirement: 方案格式為 .slnx，template.json 不宣告 guids
因方案檔已改用 `.slnx`（不含專案 GUID 與 `GlobalSection`），`template.json` SHALL NOT 宣告 `guids` 陣列。

#### Scenario: template.json 無 guids 設定
- **WHEN** 檢視 `vue-app-demo-dotnet10/.template.config/template.json`
- **THEN** 檔案中不存在 `guids` 欄位

#### Scenario: 產生的 .slnx 不含 GUID
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** `MyApp.slnx` 僅含 `<Solution>` 與 `<Project Path="..." />`，不含任何 GUID

---

### Requirement: 產生專案時 port 自動隨機化
`template.json` SHALL 定義五個 `generated port` symbols，range 為 181xx–185xx，與其他範本錯開。

| Symbol | replaces | range | fallback |
|--------|----------|-------|----------|
| `HttpPort` | `5159` | 18100–18199 | 18100 |
| `HttpsPort` | `7173` | 18200–18299 | 18200 |
| `IisPort` | `21655` | 18300–18399 | 18300 |
| `IisSslPort` | `44385` | 18400–18499 | 18400 |
| `SpaPort` | `23288` | 18500–18599 | 18500 |

#### Scenario: 同一次產生的 port 一致
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** 同一個產出目錄內，`launchSettings.json`、`.http` 檔、`.csproj` 的對應 port 值相同

---

### Requirement: 產生專案後顯示下一步指引
`template.json` SHALL 定義 `postActions` 顯示文字指引（上游 demo 範本沒有此設定），SHALL NOT 自動執行任何指令。

#### Scenario: 產生專案後顯示指引
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** CLI 輸出包含 `npm install` 與 `/scalar`、`/openapi/v1.json` 的文字指引

## REMOVED Requirements

### Requirement: 產生的專案具備唯一 GUID
**移除原因**：方案檔改用 `.slnx` 後已不含任何 GUID，`.csproj` / `.esproj` 亦無 GUID，此需求無對應實體可驗證。原 `template.json` 的 `guids` 清單與 `VueApp1.sln` 的實際 GUID 本就不符，等同從未生效。
