## Purpose

規範 `vue-app-demo-dotnet10` 的 `.template.config/template.json`：識別資訊、SDK 約束、名稱替換、排除清單、`.slnx` 方案格式、port 隨機化與產生後指引。

## Requirements

### Requirement: 範本可成功安裝
系統 SHALL 允許使用者透過 `dotnet new install .\vue-app-demo-dotnet10` 將此範本安裝為本機範本，安裝後可在 `dotnet new list` 中看到該範本。`identity` SHALL 為 `VueAppDemoDotnet10.CSharp`、`shortName` SHALL 為 `vue-app-demo-dotnet10`。

#### Scenario: 安裝成功
- **WHEN** 使用者在 repo 根目錄執行 `dotnet new install .\vue-app-demo-dotnet10`
- **THEN** 指令回傳成功，`dotnet new list` 中出現短名稱 `vue-app-demo-dotnet10`

#### Scenario: 重複安裝
- **WHEN** 範本已安裝，使用者再次執行 `dotnet new install .\vue-app-demo-dotnet10`
- **THEN** 指令成功完成（覆蓋安裝），不產生錯誤

#### Scenario: 與 .NET 8 版範本並存
- **WHEN** 使用者同時安裝 .NET 8 版的 `vue-app-demo` 與本範本
- **THEN** 兩者皆出現於 `dotnet new list`，short name 與 identity 皆不重複

---

### Requirement: 範本限定 .NET 10 以上的 SDK
`template.json` SHALL 宣告 `constraints`，以 `sdk-version` 約束 `[10.0-*)`，使此範本僅在 .NET 10 以上的 SDK 環境中可用。

#### Scenario: 僅有舊版 SDK 的環境
- **WHEN** 使用者在僅安裝 .NET 8/9 SDK 的機器執行 `dotnet new list`
- **THEN** `vue-app-demo-dotnet10` 不出現在可用範本清單中

---

### Requirement: 專案名稱自動替換
系統 SHALL 將所有出現 `VueApp1` 的檔名、資料夾名稱與檔案內容，替換為使用者指定的 `-n` 名稱。

#### Scenario: 方案名稱替換
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** 產生的方案檔名稱為 `MyApp.slnx`，內容中不含 `VueApp1`

#### Scenario: 命名空間替換
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** C# 檔案中的命名空間為 `MyApp.Server`，不含 `VueApp1`

#### Scenario: 小寫資料夾名稱替換
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** 前端目錄名稱為 `myapp.client`（全小寫），不含 `vueapp1`

---

### Requirement: 不納入暫存與產出檔案
系統 SHALL 在產生新專案時，排除 `bin`、`obj`、`dist`、`node_modules`、`.git`、`.vs`、`.template.config`、`*.user`、`.DS_Store`、`package-lock.json`、`pnpm-lock.yaml` 與 `logs` 等目錄與檔案。

#### Scenario: 產生的專案不含 node_modules
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** 產生目錄中不存在 `node_modules` 資料夾

#### Scenario: 產生的專案不含 bin/obj
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** 產生目錄中不存在 `bin` 或 `obj` 資料夾

---

### Requirement: 方案格式為 .slnx，template.json 不宣告 guids
因方案檔已改用 `.slnx`（不含專案 GUID 與 `GlobalSection`），`template.json` SHALL NOT 宣告 `guids` 陣列。

> 背景：.NET 8 版範本的 `guids` 清單與 `VueApp1.sln` 中的實際 GUID 並不相符，等同未生效。改用 `.slnx` 後此設定已無存在意義，保留只會誤導維護者。

#### Scenario: template.json 無 guids 設定
- **WHEN** 檢視 `vue-app-demo-dotnet10/.template.config/template.json`
- **THEN** 檔案中不存在 `guids` 欄位

#### Scenario: 產生的 .slnx 不含 GUID
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** `MyApp.slnx` 僅含 `<Solution>` 與 `<Project Path="..." />`，不含任何 GUID

---

### Requirement: 產生專案時 port 自動隨機化
`template.json` SHALL 定義五個 `generated port` symbols。range 刻意與 `vue-app-admin-dotnet10`（171xx–175xx）及 .NET 8 版範本（151xx–165xx）錯開。

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

#### Scenario: 兩次產生的 port 不同
- **WHEN** 使用者先後執行 `dotnet new vue-app-demo-dotnet10 -n AppA` 與 `-n AppB`
- **THEN** 兩個產出目錄的 HTTP port 值有高機率不同（隨機化）

---

### Requirement: 產生專案後顯示下一步指引
`template.json` SHALL 定義 `postActions` 顯示文字指引，SHALL NOT 自動執行任何指令。指引內容 SHALL 至少涵蓋前端 `npm install` 與 API 文件（Scalar / OpenAPI）位置，並合併為單一 `text` 項目。

#### Scenario: 產生專案後顯示指引
- **WHEN** 使用者執行 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** CLI 輸出包含 `npm install` 與 `/scalar`、`/openapi/v1.json` 的文字指引
