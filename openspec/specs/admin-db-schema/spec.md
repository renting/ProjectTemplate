## Purpose

規範範本隨附的 SQL Server schema 與 seed 資料：資料表組成、命名與稽核慣例、MS_Description、以及不得包含的領域專屬欄位。

## Requirements

### Requirement: 範本隨附資料庫 schema 檔案
範本 SHALL 在 `db/schema.sql` 提供完整的 SQL Server schema,包含以下 11 張資料表:`Basic_Users`、`Basic_Groups`、`Basic_Users_Groups`、`Basic_Modules`、`Basic_Group_Modules`、`Basic_LoginLog`、`Basic_OperationLog`、`Basic_Api_Log`、`Basic_Api_Change_Log`、`Para_Categories`、`Para_Info`,可於空白資料庫上一次執行完成建置。

#### Scenario: 在空白資料庫執行 schema.sql
- **WHEN** 使用者在新建立的空白 SQL Server 資料庫執行 `db/schema.sql`
- **THEN** 全部 11 張資料表、預設值條件約束、外鍵與擴充屬性建立成功,無錯誤

#### Scenario: schema.sql 可被 SSMS 直接執行
- **WHEN** 使用者以 SSMS 開啟 `db/schema.sql`
- **THEN** 檔案為批次(`GO`)分隔的 T-SQL,不依賴 sqlcmd 專屬語法即可執行

### Requirement: 資料表遵循 Basic_*/Para_* 命名與稽核慣例
schema 中的資料表 SHALL 遵循以下慣例:系統基礎表使用 `Basic_` 前綴、參數表使用 `Para_` 前綴;主檔類資料表包含稽核四欄位 `CreatedUserGuid (uniqueidentifier)`、`CreatedTime (datetime, DEFAULT getdate())`、`ModifiedUserGuid`、`ModifiedTime`;狀態類欄位使用 int 並於 MS_Description 列舉值意義。

#### Scenario: 主檔資料表包含稽核欄位
- **WHEN** 檢視 `Basic_Groups`、`Basic_Modules`、`Basic_Group_Modules`、`Basic_Users`、`Para_Info` 的欄位定義
- **THEN** 每張表皆包含 `CreatedUserGuid`、`CreatedTime`、`ModifiedUserGuid`、`ModifiedTime` 四欄位,且 `CreatedTime` 有 `getdate()` 預設值

#### Scenario: 狀態欄位以描述列舉值
- **WHEN** 檢視任一狀態類欄位(如 `Basic_Groups.Status`)的 MS_Description
- **THEN** 描述文字包含列舉值對照(如「0:停用、1:啟用」)

### Requirement: 所有資料表與欄位掛 MS_Description 擴充屬性
schema SHALL 以 `sys.sp_addextendedproperty` 為每張資料表與每個欄位設定繁體中文的 `MS_Description`。

#### Scenario: 欄位描述完整
- **WHEN** 於資料庫查詢 `fn_listextendedproperty` 檢視任一資料表
- **THEN** 該資料表本身與其全部欄位皆有非空的 `MS_Description`

### Requirement: schema 不含領域專屬欄位
schema SHALL NOT 包含來源專案的領域專屬欄位:`EOfficeAccount`、`EGovernmentAccount`、`UnitCode`、`Department` 不得存在;`Basic_LoginLog.LoginMethod` 的 MS_Description SHALL 使用通用文字,不得出現特定登入服務名稱。`IdNumber` 欄位暫時保留,但 SHALL 於 `schema.sql` 中以 `TODO` 註解標記待議。

#### Scenario: 已移除領域專屬欄位
- **WHEN** 於 `db/schema.sql` 全文搜尋 `EOfficeAccount`、`EGovernmentAccount`、`UnitCode`、`Department`
- **THEN** 無任何符合結果

#### Scenario: IdNumber 標記 TODO
- **WHEN** 檢視 `db/schema.sql` 中 `Basic_Users.IdNumber` 欄位定義處
- **THEN** 存在 `TODO` 註解說明此欄位為敏感個資、是否保留待後續決定

### Requirement: 密碼欄位為 BCrypt 雜湊格式
`Basic_Users` SHALL 使用 `PasswordHash varchar(72)` 儲存 BCrypt 雜湊,SHALL NOT 存在明碼密碼欄位(如 `NormalPassword varchar(30)`)。

#### Scenario: 密碼欄位型別
- **WHEN** 檢視 `Basic_Users` 資料表定義
- **THEN** 存在 `PasswordHash varchar(72)` 欄位,且不存在名為 `NormalPassword` 的欄位

### Requirement: 修正來源 schema 的結構性錯誤
schema SHALL 修正來源版本的下列錯誤:`Basic_LoginLog` 不得存在自我參照外鍵;`Basic_Users.Title` 不得有 `getdate()` 預設值;`Basic_Users_Groups` SHALL 以 `(GroupId, UserGuid)` 為複合主鍵。

#### Scenario: Basic_Users_Groups 防止重複關聯
- **WHEN** 對 `Basic_Users_Groups` 插入相同 `(GroupId, UserGuid)` 兩次
- **THEN** 第二次插入因主鍵違反而失敗

#### Scenario: LoginLog 無自我參照外鍵
- **WHEN** 檢視 `Basic_LoginLog` 的外鍵清單
- **THEN** 不存在參照自身 `Id` 欄位的外鍵

### Requirement: 範本隨附 seed 資料
範本 SHALL 在 `db/seed.sql` 提供初始資料:一筆 admin 使用者(`PasswordHash` 為文件化預設密碼的 BCrypt 雜湊)、對齊前端現有路由的 `Basic_Modules` 選單樹、一個擁有全部功能權限的管理者群組(含 `Basic_Group_Modules` 五種權限旗標與 `Basic_Users_Groups` 關聯),以及 `Para_Categories` / `Para_Info` 的示範列舉資料。

#### Scenario: seed 可在 schema 之後直接執行
- **WHEN** 使用者於執行完 `db/schema.sql` 的資料庫接續執行 `db/seed.sql`
- **THEN** 執行成功,`Basic_Users` 至少一筆 admin、`Basic_Modules` 包含前端現有路由對應的選單、admin 所屬群組擁有全部模組的五種權限

#### Scenario: 預設密碼有文件化
- **WHEN** 檢視 `db/seed.sql` 與範本 README
- **THEN** 明確記載 admin 預設密碼與「正式環境必須更改」的警語
