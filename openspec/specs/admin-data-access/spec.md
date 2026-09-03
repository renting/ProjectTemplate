## Purpose

規範資料存取層的技術選型（Dapper，不使用 EF Core）與 in-memory 骨架中 TODO SQL 的完整度要求。

## Requirements

### Requirement: Dapper 作為資料存取層
應用程式 SHALL 使用 Dapper 作為資料存取套件，不使用 EF Core。

#### Scenario: NuGet 套件
- **WHEN** 設定專案相依
- **THEN** `VueAppAdmin.Server.csproj` SHALL 包含 `Dapper` NuGet 套件參考

---

### Requirement: IDbConnection 集中管理
`IDbConnection` 的建立與註冊 SHALL 集中在 `Shared/Database/DatabaseExtensions.cs`，以 Scoped lifetime 注入。

#### Scenario: IDbConnection 註冊
- **WHEN** 應用程式啟動
- **THEN** `DatabaseExtensions.cs` 的擴充方法 SHALL 將 `IDbConnection` 註冊為 Scoped，connection string 來自 `appsettings.json` 的 `ConnectionStrings:Default`

#### Scenario: Scoped lifetime
- **WHEN** 同一個 HTTP request 內多個 Repository 使用 IDbConnection
- **THEN** SHALL 共用同一個 IDbConnection 實例（Scoped），不得使用 Transient

---

### Requirement: Repository Interface 與實作
每個 Feature SHALL 定義自己的 Repository Interface 與實作，放在該 Feature 資料夾內，SQL 語法 SHALL 集中在 Repository 實作中。

#### Scenario: Repository 命名規範
- **WHEN** 為 Auth Feature 建立資料存取
- **THEN** SHALL 建立 `IUserRepository.cs` 與 `UserRepository.cs`，放在 `Features/Auth/` 下

#### Scenario: SQL 集中在 Repository
- **WHEN** 需要查詢資料庫
- **THEN** SQL 語句 SHALL 只出現在 Repository 實作中，Service 不得直接使用 `IDbConnection` 或撰寫 SQL

#### Scenario: Service 注入 Repository Interface
- **WHEN** Service 需要存取資料
- **THEN** SHALL 注入 Repository Interface（如 `IUserRepository`），不得注入 concrete Repository class 或 `IDbConnection`

---

### Requirement: Repository 在 DI 的註冊方式
Repository 的 DI 註冊 SHALL 由所屬 Feature 的擴充方法負責，與 Service 一起在 `AddXFeature()` 中註冊。

#### Scenario: Feature 內 Repository 註冊
- **WHEN** `AddAuthFeature()` 擴充方法被呼叫
- **THEN** 除了 `IAuthService → AuthService` 之外，SHALL 同時註冊 `IUserRepository → UserRepository`（Scoped）

---

### Requirement: ExampleItemsService 維持 In-Memory 實作
範例用途的 `ExampleItemsService` 在 template 階段 SHALL 維持 in-memory 資料，Repository 骨架可先建立 Interface，實作保留 in-memory。

#### Scenario: 範例資料不連 DB
- **WHEN** 呼叫 `GET /api/exampleitems`
- **THEN** 資料 SHALL 來自 in-memory 靜態清單，不需真實資料庫連線

---

### Requirement: In-memory 骨架的 TODO 註解提供對應 schema 的完整 SQL
維持 in-memory dummy 實作的資料存取點,其 TODO 註解 SHALL 包含對應 `db/schema.sql` 資料表的完整、可直接使用的 Dapper SQL,而非示意片段。涵蓋範圍:`Features/Auth/UserRepository.cs`(登入驗證與顯示名稱查詢,對應 `Basic_Users`,含 `Status` 與鎖定欄位判斷)、`Features/Auth/GroupFeatureStore.cs`(使用者功能權限查詢,對應 `Basic_Users_Groups` JOIN `Basic_Group_Modules` JOIN `Basic_Modules`)、`Features/Menu/MenuService.cs`(選單樹查詢,對應 `Basic_Modules`,含 `FatherModuleId` 組樹說明)。runtime 行為 SHALL 維持 in-memory,不建立實際資料庫連線。

#### Scenario: TODO SQL 與 schema 欄位一致
- **WHEN** 比對 `UserRepository.cs`、`GroupFeatureStore.cs`、`MenuService.cs` 註解中的 SQL 與 `db/schema.sql` 的資料表定義
- **THEN** SQL 引用的資料表名與欄位名皆存在於 schema 中,無拼字或欄位不一致

#### Scenario: runtime 行為不變
- **WHEN** 在未建立任何資料庫的環境啟動後端並呼叫登入、選單、功能清單 API
- **THEN** 行為與變更前相同(in-memory dummy 資料),不嘗試連線資料庫

#### Scenario: 既有測試不受影響
- **WHEN** 執行 `VueAppAdmin.Server.Tests` 全部測試
- **THEN** 測試全數通過,無需修改任何測試
