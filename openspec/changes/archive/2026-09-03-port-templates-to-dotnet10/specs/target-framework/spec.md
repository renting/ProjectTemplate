## ADDED Requirements

### Requirement: 所有 .NET 專案統一以 net10.0 為目標框架
兩個範本中的每一個 `.csproj` SHALL 將 `TargetFramework` 設為 `net10.0`，不得出現多版本並存的情況。

> 背景：.NET 8 版範本中 Server 與 DbScriptExporter 為 `net8.0`、測試專案卻是 `net9.0`，版本不一致。本版一律統一。

#### Scenario: 全部專案為 net10.0
- **WHEN** 於 repo 中搜尋所有 `.csproj` 的 `<TargetFramework>` 元素
- **THEN** 每一個值皆為 `net10.0`，無 `net8.0`、`net9.0` 殘留

#### Scenario: 產出專案可用 .NET 10 SDK 建置
- **WHEN** 使用者以 .NET 10 SDK 對產出專案執行 `dotnet build`
- **THEN** 建置成功，0 錯誤 0 警告

---

### Requirement: ASP.NET Core 相依套件對齊 10.x
Server 專案的 ASP.NET Core 相關 `PackageReference` SHALL 使用 10.x 版本區間：`Microsoft.AspNetCore.Authentication.JwtBearer` 為 `10.*`、`Microsoft.AspNetCore.OpenApi` 為 `10.*`、`Microsoft.AspNetCore.SpaProxy` 為 `10.*-*`（允許 prerelease，SpaProxy 僅發佈 prerelease 通道時仍可解析）。

#### Scenario: 套件還原成功
- **WHEN** 於產出專案執行 `dotnet restore`
- **THEN** 上述套件皆解析到 10.x 版本，無版本衝突警告

---

### Requirement: 測試專案可在 .NET 10 下執行
`VueAppAdmin.Server.Tests` SHALL 在 `net10.0` 下以 xUnit 執行，且測試程式碼不需為框架升級而修改。

#### Scenario: 測試全數通過
- **WHEN** 於產出專案執行 `dotnet test`
- **THEN** 測試回合以 `.NETCoreApp,Version=v10.0` 執行，全部測試通過，0 失敗
