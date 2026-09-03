## MODIFIED Requirements

### Requirement: 範本可透過 dotnet new 指令安裝與使用
`vue-app-admin-dotnet10` 目錄 SHALL 包含有效的 `.template.config/template.json`，使 .NET SDK 可識別並安裝此範本。`identity` SHALL 為 `VueAppAdminDotnet10.CSharp`、`shortName` SHALL 為 `vue-app-admin-dotnet10`，與 .NET 8 版範本互不衝突，可於同一台機器並存安裝。

#### Scenario: 安裝範本
- **WHEN** 使用者執行 `dotnet new install .\vue-app-admin-dotnet10`
- **THEN** 安裝成功且無錯誤，`dotnet new list` 顯示 short name `vue-app-admin-dotnet10`

#### Scenario: 與 .NET 8 版範本並存
- **WHEN** 使用者同時安裝 `vue-app-admin-dotnet8` 與 `vue-app-admin-dotnet10`
- **THEN** 兩者皆出現於 `dotnet new list`，short name 與 identity 皆不重複，安裝過程無覆蓋警告

---

### Requirement: 產生專案時 port 自動隨機化
`template.json` SHALL 定義五個 `generated port` symbols，range 由 161xx–165xx 改為 171xx–175xx，與 `vue-app-demo-dotnet10`（181xx–185xx）及 .NET 8 版範本（151xx–165xx）錯開。

| Symbol | replaces | range | fallback |
|--------|----------|-------|----------|
| `HttpPort` | `5159` | 17100–17199 | 17100 |
| `HttpsPort` | `7173` | 17200–17299 | 17200 |
| `IisPort` | `21655` | 17300–17399 | 17300 |
| `IisSslPort` | `44385` | 17400–17499 | 17400 |
| `SpaPort` | `23288` | 17500–17599 | 17500 |

受影響的 template 檔案清單一併修正為實際情形：`launchSettings.json`、`.http`、`.csproj`、`VueAppAdmin.Server/README.md`、`vueappadmin.client/vite.config.ts`。

> 原規格的 port 表寫 50100–50599，與實際 `template.json` 不符；受影響檔案清單則列了不存在的 `vueappadmin.client/.vscode/launch.json`。

#### Scenario: 不同次產生的 port 不同
- **WHEN** 使用者兩次執行 `dotnet new vue-app-admin-dotnet10 -n MyApp` 於不同目錄
- **THEN** 兩個產出目錄中的 `launchSettings.json` HTTP port 值有高機率不同（隨機化）

#### Scenario: 同一次產生的 port 一致
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet10 -n MyApp`
- **THEN** 同一個產出目錄內，`launchSettings.json`、`.http` 檔、`.csproj` 的對應 port 值相同

## ADDED Requirements

### Requirement: 範本限定 .NET 10 以上的 SDK
`template.json` SHALL 宣告 `constraints`，以 `sdk-version` 約束 `[10.0-*)`，使此範本僅在 .NET 10 以上的 SDK 環境中可用。

#### Scenario: .NET 10 SDK 環境
- **WHEN** 使用者在安裝 .NET 10 SDK 的機器執行 `dotnet new list`
- **THEN** `vue-app-admin-dotnet10` 出現在清單中

#### Scenario: 僅有舊版 SDK 的環境
- **WHEN** 使用者在僅安裝 .NET 8/9 SDK 的機器執行 `dotnet new list`
- **THEN** `vue-app-admin-dotnet10` 不出現在可用範本清單中，避免產生無法建置的專案

## REMOVED Requirements

### Requirement: `.vscode/launch.json` 僅保留 chrome configuration
**移除原因**：`vueappadmin.client/.vscode/` 下只有 `extensions.json`，`launch.json` 從未存在於範本中，此需求無對應實體可驗證。
