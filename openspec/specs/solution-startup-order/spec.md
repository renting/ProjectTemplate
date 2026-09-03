## Purpose

規範兩個範本的方案檔格式（`.slnx`）與專案宣告順序，確保 Visual Studio 預設以 Server 為 startup project、SpaProxy 能連帶啟動前端。

## Requirements

### Requirement: 兩個範本的方案檔皆使用 .slnx 格式
`vue-app-admin-dotnet10` 與 `vue-app-demo-dotnet10` SHALL 一律提供 `.slnx`（XML 格式）方案檔，不得保留舊的 `.sln` 格式。

`.slnx` 不含專案 GUID 與 `GlobalSection`，因此 `template.json` **不需要**（也不應該）宣告 `guids` 陣列。

#### Scenario: 產生目錄僅含 .slnx
- **WHEN** 使用者執行 `dotnet new vue-app-admin-dotnet10 -n MyApp` 或 `dotnet new vue-app-demo-dotnet10 -n MyApp`
- **THEN** 產生目錄中存在 `MyApp.slnx`，且不存在 `MyApp.sln`

#### Scenario: .slnx 可被 dotnet CLI 建置
- **WHEN** 使用者於產生目錄執行 `dotnet build MyApp.slnx`
- **THEN** 建置成功（exit code 0），CLI 正確識別 `.slnx` 並建置其中所有專案

---

### Requirement: Server 為方案預設 startup project
範本產出的 `.slnx` 中，Server `.csproj` 的 `<Project>` 元素 SHALL 排列於 client `.esproj` 之前，使 Visual Studio 在無 `.suo` 時預設以 Server 作為 startup project。

#### Scenario: 首次開啟 .slnx（無 .suo）
- **WHEN** 使用者以 VS 開啟 `dotnet new` 產出的 `.slnx`，且同目錄無 `.suo` 檔
- **THEN** VS 綠色執行箭頭指向 Server `.csproj`，而非 client `.esproj`

#### Scenario: SpaProxy 自動啟動前端
- **WHEN** 使用者按下綠色箭頭執行 Server `.csproj`
- **THEN** SpaProxy 自動在 `SpaRoot` 目錄執行 `SpaProxyLaunchCommand` 指定的指令（admin 為 `pnpm run dev`、demo 為 `npm run dev`），前端 dev server 一併啟動

#### Scenario: 排序不影響建置結果
- **WHEN** `.slnx` 中 `<Project>` 宣告順序調整後
- **THEN** `dotnet build` 仍建置相同的專案集合，建置行為不變
