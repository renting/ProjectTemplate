## Purpose

規範後端共用工具與示範性分層程式碼（密碼雜湊、ExampleItems 的 Controller/Service 分層）的存在與行為。

## Requirements

### Requirement: PasswordHasherHelper 提供密碼雜湊工具
`PasswordHasherHelper.cs` SHALL 提供 `HashPassword(plainText)` 與 `VerifyPassword(plainText, hash)` 靜態方法，使用 BCrypt 或 ASP.NET Core 內建 `PasswordHasher<T>`。

#### Scenario: 雜湊後可驗證
- **WHEN** `HashPassword("myPassword")` 產生 hash，再以 `VerifyPassword("myPassword", hash)` 驗證
- **THEN** 回傳 `true`

---

### Requirement: ExampleItems 展示 Controller/Service 分層
`ExampleItemsController.cs` 與 `ExampleItemsService.cs` SHALL 作為示範用分層架構範例，使用 hardcoded dummy data（無 DB），提供 `POST /api/ExampleItems/Search`（分頁搜尋）與 `GET /api/ExampleItems/{id:int}`（單筆）。

> 搜尋端點的查詢參數、分頁與排序行為詳見 `example-items-search` capability；本 capability 只約束分層結構與端點存在性。

#### Scenario: 搜尋端點回傳分頁包裝
- **WHEN** 呼叫 `POST /api/ExampleItems/Search`（已登入）
- **THEN** 回應 HTTP 200，body 為 `ApiPagedResponse<ItemResponse>`，含 `results` 集合與 `total` 總筆數

#### Scenario: 取得單筆 ExampleItem
- **WHEN** 呼叫 `GET /api/ExampleItems/1`（已登入）
- **THEN** 回應 HTTP 200，body 為 `ApiResponse<ItemResponse>` 單筆物件

#### Scenario: 不存在的 id
- **WHEN** 呼叫 `GET /api/ExampleItems/999`（已登入）
- **THEN** 回應 HTTP 404，body 為 `ApiResponse<object>.Fail("找不到指定項目")`
