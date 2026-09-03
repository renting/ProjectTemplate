# DbScriptExporter

命令列工具，模擬 SSMS「產生指令碼」精靈（右鍵資料庫 → 工作 → 產生指令碼 → 編寫整個資料庫和所有資料庫物件的指令碼 → 儲存為指令檔、每個物件一個指令檔），使用 SMO（`Microsoft.SqlServer.SqlManagementObjects`）列舉資料庫物件並輸出為 `.sql` 檔。

---

## 專案結構

```
DbScriptExporter/
├── Program.cs                  # 進入點：解析命令列參數、建立 SMO 連線、呼叫匯出、印總結
├── DatabaseScriptExporter.cs   # 核心匯出邏輯：走訪各物件集合、跳過系統物件、逐一產生指令碼
├── ScriptFileNameHelper.cs     # 檔名規則（含 Trigger 特例）與不合法字元過濾
└── DbScriptExporter.csproj
```

---

## 啟動

```bash
# 在方案根目錄（含 .slnx）時，需指定專案路徑
dotnet run --project .\DbScriptExporter\ -- <ServerName> <DatabaseName> <輸出資料夾> [UserId] [Password]

# 已在 DbScriptExporter/ 目錄內時可直接執行
dotnet run -- <ServerName> <DatabaseName> <輸出資料夾> [UserId] [Password]
```

> 建議將輸出資料夾指定為 `..\db\scripts`，與同層的 `db/schema.sql`、`db/seed.sql` 放在一起，方便對照資料庫現況與範本內建的 schema/seed。

### 命令列參數

| 參數 | 必要 | 說明 |
|------|------|------|
| `ServerName` | 是 | SQL Server 執行個體名稱 |
| `DatabaseName` | 是 | 目標資料庫名稱 |
| 輸出資料夾 | 是 | 指令碼輸出的根目錄，不存在時會自動建立 |
| `UserId` | 否 | SQL Server 驗證帳號；未提供時使用 Windows 整合式驗證 (`LoginSecure`) |
| `Password` | 否 | SQL Server 驗證密碼；僅在提供 `UserId` 時使用 |

---

## 使用範例

```bash
# Windows 整合式驗證
dotnet run --project .\DbScriptExporter\ -- localhost SMO .\db\scripts

# SQL Server 驗證（密碼會出現在 shell 歷史紀錄與程序列表，僅建議本機臨時測試使用；
# 正式/共用環境請改用 Windows 整合式驗證或於執行時互動輸入）
dotnet run --project .\DbScriptExporter\ -- localhost SMO .\db\scripts sa P@ssw0rd
```

執行後於 Console 可看到每個物件的產出進度：

```
連線至 'localhost'，資料庫 'SMO'，驗證方式：Windows 整合式驗證
輸出資料夾：D:\horse\SMO\output

[Schemas] dbo -> Schemas/dbo.dbo.sql
[Tables] dbo.Basic_Users -> Tables/dbo.Basic_Users.sql
[Views] dbo.V_UserMenu -> Views/dbo.V_UserMenu.sql
[StoredProcedures] dbo.usp_GetUser -> StoredProcedures/dbo.usp_GetUser.sql
[Triggers] dbo.Basic_Users.trg_Users_Audit -> Triggers/dbo.Basic_Users.trg_Users_Audit.sql
[WARN] 略過 Table dbo.Legacy_Table：物件已損毀，無法產生指令碼

===== 匯出完成 =====
總物件數：42
成功：41
失敗：1
輸出資料夾：D:\horse\SMO\output
```

---

## 輸出結構與檔名規則

依物件類型分類輸出至以下子資料夾，每個物件輸出一個 `.sql` 檔：

| 子資料夾 | 物件類型 |
|----------|----------|
| `Tables/` | 資料表 |
| `Views/` | 檢視表 |
| `StoredProcedures/` | 預存程序 |
| `Functions/` | 使用者自訂函數 |
| `Triggers/` | 資料表觸發器（Table-level） |
| `TableTypes/` | 使用者自訂資料表類型 |
| `DataTypes/` | 使用者自訂資料類型 |
| `Sequences/` | 序列 |
| `Synonyms/` | 同義字 |
| `Schemas/` | 結構描述（Schema） |

檔名規則：
- 一般物件：`{Schema}.{ObjectName}.sql`
- Trigger：`{Schema}.{TableName}.{TriggerName}.sql`
- 檔名中的作業系統不合法字元會被替換，避免寫檔失敗。

系統物件（`IsSystemObject == true`）不會被匯出。

---

## 指令碼產生選項

沿用以下 `ScriptingOptions`，使產出內容對齊 SSMS「產生指令碼」精靈：

`IncludeIfNotExists`、`Indexes`、`Triggers`、`DriPrimaryKey`、`DriForeignKeys`、`DriUniqueKeys`、`DriChecks`、`DriDefaults`、`ExtendedProperties`、`SchemaQualify`、`ToFileOnly`

---

## 錯誤處理

單一物件產生指令碼失敗時，僅於 Console 印出 `[WARN]` 警告訊息並繼續處理下一個物件，不會中斷整體匯出流程。程式結束時會印出總結（處理物件總數、成功數、失敗數），若有失敗物件，處理程序 (process) 結束碼為 `1`。

---

## 套件版本

| 套件 | 版本 |
|------|------|
| `Microsoft.SqlServer.SqlManagementObjects` | `181.25.0` |
| `Microsoft.Data.SqlClient` | `7.0.1`（對齊 `SMO.Server.csproj`） |
