## Purpose

規範 `vue-app-admin-dotnet10` 的 `.template.config/template.json`：識別資訊、SDK 約束、名稱替換、排除清單、port 隨機化、JWT secret 唯一化與產生後指引。

## Requirements

### Requirement: 範本可透過 dotnet new 指令安裝與使用
`vue-app-admin-dotnet10` 目錄 SHALL 包含有效的 `.template.config/template.json`，使 .NET SDK 可識別並安裝此範本。`identity` SHALL 為 `VueAppAdminDotnet10.CSharp`、`shortName` SHALL 為 `vue-app-admin-dotnet10`，與 .NET 8 版範本互不衝突，可於同一台機器並存安裝。

#### Scenario: 安裝範本
- **WHEN** 使用者執行 `dotnet new install .\vue-app-admin-dotnet10`
- **THEN** 安裝成功且無錯誤，`dotnet new list` 顯示 short name `vue-app-admin-dotnet10`

#### Scenario: 與 .NET 8 版範本並存
- **WHEN** 使用者同時安裝 `vue-app-admin-dotnet8` 與 `vue-app-admin-dotnet10`
- **THEN** 兩者皆出現於 `dotnet new list`，short name 與 identity 皆不重複，安裝過程無覆蓋警告

#### Scenario: 產生新專案
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet10 -n MyApp`
- **THEN** 在目前目錄下產生完整的方案目錄 `MyApp/`

---

### Requirement: 範本限定 .NET 10 以上的 SDK
`template.json` SHALL 宣告 `constraints`，以 `sdk-version` 約束 `[10.0-*)`，使此範本僅在 .NET 10 以上的 SDK 環境中可用。

#### Scenario: .NET 10 SDK 環境
- **WHEN** 使用者在安裝 .NET 10 SDK 的機器執行 `dotnet new list`
- **THEN** `vue-app-admin-dotnet10` 出現在清單中

#### Scenario: 僅有舊版 SDK 的環境
- **WHEN** 使用者在僅安裝 .NET 8/9 SDK 的機器執行 `dotnet new list`
- **THEN** `vue-app-admin-dotnet10` 不出現在可用範本清單中，避免產生無法建置的專案

---

### Requirement: sourceName 替換覆蓋所有檔案
`template.json` 的 `sourceName` SHALL 設為 `VueAppAdmin`，SDK 範本引擎 SHALL 將所有出現 `VueAppAdmin` 的地方（檔名、資料夾名、命名空間、檔案內容）替換為使用者指定的 `-n` 名稱。

#### Scenario: 後端命名空間替換
- **WHEN** 使用者以 `-n MyApp` 產生專案
- **THEN** 所有 `.cs` 檔案中的命名空間由 `VueAppAdmin` 替換為 `MyApp`

#### Scenario: 方案與專案檔名替換
- **WHEN** 使用者以 `-n MyApp` 產生專案
- **THEN** `.slnx`、`.csproj`、`.esproj` 檔名均包含 `MyApp` 而非 `VueAppAdmin`

#### Scenario: 產出專案不殘留範本名稱
- **WHEN** 使用者以 `-n MyApp` 產生專案後，於產出目錄全文搜尋 `VueAppAdmin`
- **THEN** `.cs`、`.csproj`、`.slnx`、`.ts` 等檔案中無任何符合結果

---

### Requirement: 前端目錄名稱以小寫替換
`template.json` SHALL 設定 `nameLower` derived symbol（`lowerCase` transform）加上 `fileRename: "vueappadmin"`，使前端目錄名稱正確替換為全小寫形式。

#### Scenario: 前端目錄名稱小寫
- **WHEN** 使用者以 `-n MyApp` 產生專案
- **THEN** 前端目錄名稱為 `myapp.client`（全小寫），而非 `MyApp.client`

---

### Requirement: 排除不應納入範本的檔案
`template.json` 的 `sources` 排除清單 SHALL 包含 `.git`、`.vs`、`.template.config`、`bin`、`obj`、`dist`、`node_modules`、`*.user`、`.DS_Store`、`package-lock.json`、`pnpm-lock.yaml`、`pnpm-workspace.yaml`、`logs` 等目錄與檔案。

#### Scenario: 產生目錄不含建置產物
- **WHEN** 使用者產生新專案
- **THEN** 產生目錄中不含 `bin/`、`obj/`、`node_modules/`、`.vs/`、`dist/`、`logs/` 等目錄

#### Scenario: 產生目錄不含 lock file
- **WHEN** 使用者產生新專案
- **THEN** 產生目錄中不含 `package-lock.json` 或 `pnpm-lock.yaml`

---

### Requirement: 產生專案時 port 自動隨機化
`template.json` SHALL 定義五個 `generated port` symbols，使每次 `dotnet new` 時各個 port 產生不重疊的隨機值，並替換所有相關 template 檔案中的對應數字。range 刻意與 `vue-app-demo-dotnet10`（181xx–185xx）及 .NET 8 版範本（151xx–165xx）錯開。

Symbol 定義：

| Symbol | replaces | range | fallback |
|--------|----------|-------|----------|
| `HttpPort` | `5159` | 17100–17199 | 17100 |
| `HttpsPort` | `7173` | 17200–17299 | 17200 |
| `IisPort` | `21655` | 17300–17399 | 17300 |
| `IisSslPort` | `44385` | 17400–17499 | 17400 |
| `SpaPort` | `23288` | 17500–17599 | 17500 |

受影響的 template 檔案（`replaces` 字串必須出現於這些檔案中）：
- `VueAppAdmin.Server/Properties/launchSettings.json`
- `VueAppAdmin.Server/VueAppAdmin.Server.http`
- `VueAppAdmin.Server/VueAppAdmin.Server.csproj`（`SpaProxyServerUrl`）
- `VueAppAdmin.Server/README.md`（API 文件的 URL 範例）
- `vueappadmin.client/vite.config.ts`（後端 target 與 dev server port 的預設值）

#### Scenario: 不同次產生的 port 不同
- **WHEN** 使用者兩次執行 `dotnet new vue-app-admin-dotnet10 -n MyApp` 於不同目錄
- **THEN** 兩個產出目錄中的 `launchSettings.json` HTTP port 值有高機率不同（隨機化）

#### Scenario: 同一次產生的 port 一致
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet10 -n MyApp`
- **THEN** 同一個產出目錄內，`launchSettings.json`、`.http` 檔、`.csproj` 的對應 port 值相同

---

### Requirement: JWT secret 於產生專案時自動唯一化
`template.json` SHALL 定義 `JwtSecret` symbol（`generated` guid，`defaultFormat: "n"`），置換 `appsettings.json` 與 `appsettings.Development.json` 中 `Jwt:SignKey` 的佔位字串；`Jwt:SignKey` 的範本值 SHALL 為「`VueAppAdmin` + 分隔符 + 佔位字串」形式，使產出專案的 SignKey 成為「專案名稱 + 隨機 GUID」且總長度大於 32 字元。

#### Scenario: 產出專案的 SignKey 含專案名稱與隨機值
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet10 -n MyApp`
- **THEN** 產出的 `appsettings.json` 中 `Jwt:SignKey` 以 `MyApp-` 開頭、後接 32 位小寫十六進位字串，且不含原佔位字串

#### Scenario: 不同次產生的 SignKey 不同
- **WHEN** 使用者兩次執行 `dotnet new vue-app-admin-dotnet10 -n MyApp` 於不同目錄
- **THEN** 兩個產出目錄的 `Jwt:SignKey` 值不同

---

### Requirement: 產生專案後顯示下一步指引
`template.json` SHALL 定義 `postActions`，於 `dotnet new` 產生專案完成後顯示文字指引（`actionId: AC1156F7-BB77-4DB8-B28F-24EEBCCA1E5C`），SHALL NOT 自動執行任何指令（不使用 run script、restore 等會實際執行動作的 action）。

指引內容 SHALL 至少涵蓋：前端 `pnpm install`（含未安裝 pnpm 時的提示）、in-memory 示範帳密與更改提醒、API 文件（Scalar / OpenAPI）位置、以及選用的資料庫建置步驟（`db/schema.sql`、`db/seed.sql` 與 `ConnectionStrings:Default`）。

因目前 SDK 版本的 `manualInstructions` 陣列只會顯示第一項，全部步驟 SHALL 合併為單一 `text` 項目。

#### Scenario: 產生專案後顯示指引
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet10 -n MyApp`
- **THEN** CLI 輸出包含 pnpm install、示範帳密、API 文件位置、schema/seed 等後續步驟的文字指引

#### Scenario: 不自動執行指令
- **WHEN** 使用者於無 SQL Server、無 Node.js 的環境產生專案
- **THEN** 專案產生成功，不因 postActions 嘗試執行外部指令而失敗

---

### Requirement: db 資料夾納入範本輸出
範本 `sources` 設定 SHALL 使 `db/schema.sql` 與 `db/seed.sql` 包含於產出專案中。

#### Scenario: 產出專案含 db 資料夾
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet10 -n MyApp`
- **THEN** 產出目錄包含 `db/schema.sql` 與 `db/seed.sql`
