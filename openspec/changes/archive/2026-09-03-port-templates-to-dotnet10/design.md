## Context

來源為 `littlehorseboy/dotnet-new-templates`（.NET 8），含兩個 `dotnet new` 範本：

- `vue-app-admin-dotnet8`：完整後台骨架（JWT、Serilog、Dapper、db schema、xUnit、PrimeVue）
- `vue-app-demo`：VS 內建「Vue 和 ASP.NET Core」專案類型的擴充版

驗證環境：Windows 11、.NET SDK 10.0.202（同機另有 3.1 / 6.0 / 9.0）、Node.js v24。

## Goals / Non-Goals

**Goals:**
- 兩個範本全部專案統一 `net10.0`，可用 .NET 10 SDK 建置並通過既有測試
- API 文件改用 .NET 10 內建 OpenAPI，且不喪失 JWT Bearer 的「Authorize」能力
- 與 .NET 8 版範本同機並存不衝突
- 修正移植過程中發現的既有規格漂移（stale spec）

**Non-Goals:**
- 不升級前端相依（Vue / Vite / PrimeVue / axios 等維持來源版本）
- 不改動業務邏輯、API 合約、db schema、log 格式
- 不改寫測試框架（維持 xUnit v2，不遷移到 xunit.v3 / Microsoft.Testing.Platform）
- 不搬移上游的 openspec 歷史 archive（見決策 4）

## Decisions

### 決策 1：API 文件改用內建 OpenAPI + Scalar，而非沿用 Swashbuckle

**選項 A（採用）**：`Microsoft.AspNetCore.OpenApi` 產生文件（OpenAPI 3.1）+ `Scalar.AspNetCore` 提供 UI。

**選項 B**：升級 `Swashbuckle.AspNetCore` 6.6.2 → 10.2.3，保留 Swagger UI。

**選 A 的理由**：
- .NET 9 起官方 Web API 範本已移除 Swashbuckle，選 B 會讓範本長期偏離官方預設
- 內建產生器隨 SDK 更新，少一個第三方相依的版本追蹤負擔
- OpenAPI 3.1 對 nullable 等型別的表達較 3.0 精確

**代價**：失去 Swagger UI 的熟悉度；`AddSecurityDefinition` / `AddSecurityRequirement` 需自行以 transformer 重寫（見決策 2）。

---

### 決策 2：JWT security scheme 以 `IOpenApiDocumentTransformer` 實作

內建 OpenAPI 沒有 Swashbuckle 的 security 設定 API。新增 `Shared/OpenApi/BearerSecuritySchemeTransformer.cs`：

- 注入 `IAuthenticationSchemeProvider`，先確認確實註冊了名為 `Bearer` 的 scheme 才修改文件——避免有人拆掉 JWT 後文件仍宣稱需要 token
- 寫入 `components.securitySchemes.Bearer`，並在根層 `security` 加一筆全域 requirement，對齊 `Program.cs` 全域套用的 `AuthorizeFilter`

**踩到的坑**：.NET 10 隨附的 `Microsoft.OpenApi` 已是 **v2**，`OpenApiDocument`、`OpenApiSecurityScheme`、`OpenApiComponents` 等型別從 `Microsoft.OpenApi.Models` **移到 `Microsoft.OpenApi`**；沿用舊命名空間會得到 `CS0234`。security requirement 的 key 也改用 `OpenApiSecuritySchemeReference(name, document)`。此點已寫入根目錄 README 與 `api-documentation` spec，避免下次再撞一次。

---

### 決策 3：port range 與 short name 全面錯開，讓兩代範本並存

| | .NET 8 版 | 本版 |
|---|---|---|
| admin short name | `vue-app-admin-dotnet8` | `vue-app-admin-dotnet10` |
| demo short name | `vue-app-demo` | `vue-app-demo-dotnet10` |
| admin port range | 161xx–165xx | 171xx–175xx |
| demo port range | 151xx–155xx | 181xx–185xx |

另加 `constraints.sdk-version: [10.0-*)`，讓只有舊 SDK 的機器不會看到本版範本、也就不會產出建不起來的專案。

---

### 決策 4：不搬移上游的 openspec archive，改以本次移植作為第一筆歷史

上游 archive 有 15 筆變更，內容全部指向 `vue-app-admin-dotnet8`、Swagger、`net8.0`。整批複製會讓本 repo 的歷史敘述與現況互相矛盾，違反 `config.yaml` 的「規格、proposal、tasks 之間必須一致」原則。

作法：
- `openspec/specs/` 完整承接並改寫為 .NET 10 現況（這才是維護時真正會讀的「現行事實」）
- `openspec/changes/archive/` 只放本次移植這一筆，作為工作流的起點與範例
- 移植前的沿革保留在上游 repo，README 有連結

---

### 決策 5：順手修正發現的規格漂移

移植時逐條核對規格與程式碼，修掉五處既有的不一致：

1. `template-config` 要求 `vueappadmin.client/.vscode/launch.json` 只留 chrome configuration——**該檔案根本不存在**，需求已移除
2. `template-config` 的 port 表寫 50100–50599，**與實際 `template.json` 的 161xx 不符**——本版改為表列實際值
3. `backend-architecture` 說 ExampleItems 提供 `GET /api/ExampleItems` 列表，**實際是 `POST /api/ExampleItems/Search`**（後續分頁改版造成的漂移，且與 `example-items-search` capability 互相矛盾）——已修正並註明細節歸屬
4. `backend-auth` 說 JWT 設定來自 `JwtServiceCollectionExtensions.cs`，**實際檔名是 `Shared/Jwt/JwtExtensions.cs`**——寫了一支小工具，把所有 spec 中以反引號標注的檔名跟兩個範本目錄的實際檔案對照，才抓到這一處
5. `admin-backend-auth` 與 `admin-logging` 檔案中段殘留 `## ADDED Requirements` 標頭（上游 `openspec archive` 併檔時沒清掉），**導致該標頭之後的 requirement 全部不被 CLI 解析**：logging 少算 11 條、backend-auth 少算 1 條。清掉後可解析的 requirement 由 115 條變成 127 條

前三項是敘述與程式碼不符；後兩項則是連「規格本身讀不讀得到」都出了問題——這正好印證決策 7 的判斷：驗證沒跑起來，漂移就會一直累積。

同時把上游尚未歸檔、但實作已完成（tasks 全數 `[x]`）的 `add-db-schema-and-template-ux` 併入 `openspec/specs/`（`db-schema` 新增、`data-access` 與 `template-config` 補上對應需求），等同替上游補做 `openspec archive` 這一步。

---

### 決策 6：demo 範本方案檔改用 `.slnx`

admin 範本已是 `.slnx`，demo 仍為傳統 `.sln`。改用 `dotnet sln migrate` 產生後手動調整：

- 把 Server `.csproj` 排到 client `.esproj` 之前（`solution-startup-order` 的要求，migrate 產出的順序相反）
- 移除 migrate 帶出的 `<Build />` / `<Deploy />`，與 admin 的極簡風格一致

連帶效果：`.slnx` 不含 GUID，`template.json` 的 `guids` 陣列失去意義而移除。順帶一提，上游那三個 GUID **本來就與 `VueApp1.sln` 的實際值不符**，等同從未生效。

---

### 決策 7：capability 改為單層扁平 id，並補上 `## Purpose` / `## Requirements`

搬移時實測發現上游的規格結構 **openspec CLI 根本不認**：

```
$ openspec list --specs          # 上游結構
  vue-app-admin-dotnet8    requirements 0     # 巢狀子目錄沒被展開
$ openspec validate --all
  ✗ spec/... Spec must have a Purpose section
```

CLI 只掃 `openspec/specs/<id>/spec.md` 單層，且主規格需要 `## Purpose` + `## Requirements`（`## ADDED Requirements` 是 **change delta** 的格式，不是主規格格式——上游應是 `openspec archive` 直接搬檔而沒轉格式）。

結果就是：上游的驗證從來沒真正跑起來，規格漂移（決策 5 那三處）才會長期沒被發現。

**採用**：
- capability id 扁平化並加前綴：`admin-<cap>`、`demo-<cap>`，跨範本共用的維持無前綴（`target-framework`、`solution-startup-order`）
- 每份主規格補 `## Purpose`（逐一撰寫，非樣板文字）與 `## Requirements`
- change 目錄內的 `specs/` 維持 delta 格式（`## ADDED` / `## MODIFIED` / `## REMOVED`），這才是它該有的格式

**代價**：失去目錄層級的視覺分組，id 變長（`admin-example-items-search`）。換來的是 `openspec validate --all` 與 `openspec list --specs` 真的能用——對「往後維護的基礎」而言，能驗證比好看重要。

驗證結果：25 passed / 0 failed，127 條 requirement 全部被正確解析（轉換前 CLI 只能解析到 115 條——見決策 5 第 4 點）。

## Risks / Trade-offs

- **Scalar 是第三方相依**：內建 OpenAPI 只產生文件、不提供 UI。若日後不想要第三方 UI，可只保留 `MapOpenApi()` 並以 IDE 或外部工具讀 `/openapi/v1.json`
- **SpaProxy 使用 `10.*-*`（含 prerelease）**：沿用上游作法，因該套件常只在 prerelease 通道更新；若日後有穩定版可收斂為 `10.*`
- **xUnit 維持 v2**：.NET 10 官方範本預設 xunit.v3 + Microsoft.Testing.Platform。維持 v2 讓 23 個既有測試零修改通過；日後要遷移可獨立提一次變更
- **`sdk-version` 約束**：好處是擋掉舊 SDK 誤用；代價是同機只裝舊 SDK 的人會「看不到範本」而非「看到錯誤訊息」，需靠 README 說明

## Migration Plan

一次性移植，非漸進式：來源 repo 不動，本 repo 為全新一份。使用者端無遷移成本——兩代範本 short name 不同，可同機並存，既有以 .NET 8 範本產出的專案完全不受影響。
