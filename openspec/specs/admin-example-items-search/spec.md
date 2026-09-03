## Purpose

規範 ExampleItems 的 server-side 分頁、排序、關鍵字與日期區間篩選行為，對應前端 PrimeVue DataTable 的 lazy 模式。

## Requirements

### Requirement: ItemResponse 加入類別欄位

`ItemResponse` SHALL 包含 `categoryId: int` 與 `categoryName: string`。30 筆 in-memory 資料各自指定 categoryId（1、2、3 隨意分配）。

#### Scenario: 單筆查詢包含類別資訊

- **WHEN** 呼叫 `GET /api/ExampleItems/{id}`
- **THEN** response 包含 `categoryId` 與 `categoryName` 欄位

### Requirement: POST /api/ExampleItems/Search

系統 SHALL 以 `POST /api/ExampleItems/Search`（需 JWT）取代原 `GET /api/ExampleItems`。Request body：

```json
{
  "page": 1,
  "pageSize": 10,
  "sortField": "name",
  "sortOrder": "asc",
  "name": "",
  "description": "",
  "categoryIds": [],
  "dateFrom": null,
  "dateTo": null
}
```

所有查詢條件為選填。`name` 與 `description` 為模糊查詢（不區分大小寫，contains 語意）。`categoryIds` 為複選過濾，空陣列代表不過濾。`dateFrom`、`dateTo` 為 `createdDate` 的查詢區間，語意為「`createdDate` 落在 `[dateFrom, dateTo]` 閉區間」（含兩端點）；只帶 `dateFrom` 代表「大於等於」，只帶 `dateTo` 代表「小於等於」，兩者皆未帶代表不過濾日期。回傳 `ApiPagedResponse<ItemResponse>`。

#### Scenario: 不帶條件查詢回傳全部分頁資料

- **WHEN** 呼叫 `POST /api/ExampleItems/Search` body 為 `{ "page": 1, "pageSize": 10 }`
- **THEN** 回傳第 1 頁 10 筆，total 為 30

#### Scenario: 名稱模糊查詢

- **WHEN** body 包含 `"name": "item 1"`
- **THEN** 僅回傳 name 包含 "item 1"（不分大小寫）的項目

#### Scenario: 說明模糊查詢

- **WHEN** body 包含 `"description": "desc"`
- **THEN** 僅回傳 description 包含 "desc" 的項目

#### Scenario: 類別複選過濾

- **WHEN** body 包含 `"categoryIds": [1, 3]`
- **THEN** 僅回傳 categoryId 為 1 或 3 的項目

#### Scenario: 複合條件查詢

- **WHEN** body 包含 name 模糊條件與 categoryIds
- **THEN** 回傳同時符合兩個條件的項目

#### Scenario: 日期區間查詢（起訖皆帶）

- **WHEN** body 包含 `"dateFrom": "2026-06-01", "dateTo": "2026-06-15"`
- **THEN** 僅回傳 `createdDate` 落在 2026-06-01 至 2026-06-15（含首尾）之間的項目

#### Scenario: 僅帶起始日期

- **WHEN** body 只包含 `"dateFrom": "2026-06-01"`（未帶 `dateTo`）
- **THEN** 僅回傳 `createdDate` 大於等於 2026-06-01 的項目

#### Scenario: 僅帶結束日期

- **WHEN** body 只包含 `"dateTo": "2026-06-15"`（未帶 `dateFrom`）
- **THEN** 僅回傳 `createdDate` 小於等於 2026-06-15 的項目

#### Scenario: 日期區間與其他條件複合查詢

- **WHEN** body 同時包含 `name` 模糊條件與 `dateFrom`/`dateTo`
- **THEN** 回傳同時符合名稱條件與日期區間條件的項目

### Requirement: 前端 ExampleItemsView 篩選 UI

`ExampleItemsView.vue` SHALL 在表格上方提供篩選列，包含：
- 名稱輸入框（PrimeVue `InputText`，無自動查詢）
- 說明輸入框（PrimeVue `InputText`，無自動查詢）
- PrimeVue MultiSelect 類別複選（選項來自 `POST /api/ExampleCategories`，選取後不觸發查詢）
- PrimeVue `DatePicker`（`selectionMode="range"`）用於選擇 `createdDate` 查詢區間，選取後不觸發查詢
- 「查詢」按鈕，點擊後重設至第一頁並呼叫 `POST /api/ExampleItems/Search`

篩選列排版 SHALL 容納上述五個控件，所有控件高度一致，以 PrimeVue MultiSelect 預設高度為基準。

#### Scenario: 點擊查詢按鈕才觸發 API 請求

- **WHEN** 使用者填寫任意篩選條件後點擊「查詢」按鈕
- **THEN** 前端重設分頁至第一頁，並以最新條件呼叫 `POST /api/ExampleItems/Search`

#### Scenario: 輸入文字不自動觸發查詢

- **WHEN** 使用者在名稱或說明輸入框輸入文字
- **THEN** 前端不發出任何 API 請求，直到使用者點擊「查詢」按鈕

#### Scenario: 選擇類別不自動觸發查詢

- **WHEN** 使用者在 MultiSelect 選擇或取消選擇類別
- **THEN** 前端不發出任何 API 請求，直到使用者點擊「查詢」按鈕

#### Scenario: 選擇日期區間不自動觸發查詢

- **WHEN** 使用者在 `DatePicker` 選擇起訖日期
- **THEN** 前端不發出任何 API 請求，直到使用者點擊「查詢」按鈕

#### Scenario: 在文字輸入框按 Enter 觸發查詢

- **WHEN** 使用者在名稱或說明輸入框按下 Enter 鍵
- **THEN** 前端重設分頁至第一頁，並以最新條件呼叫 `POST /api/ExampleItems/Search`

#### Scenario: 類別 MultiSelect 載入選項

- **WHEN** ExampleItemsView 頁面初始化
- **THEN** 呼叫 `POST /api/ExampleCategories` 並將結果填入 MultiSelect 選項

### Requirement: ItemResponse 加入建立日期欄位

`ItemResponse` SHALL 包含 `createdDate: DateTime` 欄位。30 筆 in-memory 假資料 SHALL 各自指定不同的 `createdDate`（以 `DateTime.Today.AddDays(-i)` 依索引遞減，避免全部同一天導致排序與區間篩選無法驗證）。

#### Scenario: 單筆查詢包含建立日期

- **WHEN** 呼叫 `GET /api/ExampleItems/{id}`
- **THEN** response 包含 `createdDate` 欄位

#### Scenario: 假資料日期彼此不同

- **WHEN** 檢視 in-memory 假資料清單
- **THEN** 30 筆資料的 `createdDate` 至少涵蓋 30 個不同日期

### Requirement: 排序支援所有欄位

`POST /api/ExampleItems/Search` 的 `sortField` SHALL 支援 `id`、`name`、`description`、`categoryName`、`createdDate` 五個欄位的排序，每個欄位皆 SHALL 同時支援 `sortOrder: "asc"` 與 `"desc"`。`sortField` 為未知字串時 SHALL fallback 為 `id` 排序（維持既有行為）。

#### Scenario: 依類別名稱排序

- **WHEN** body 包含 `"sortField": "categoryName", "sortOrder": "asc"`
- **THEN** 回傳結果依 `categoryName` 字母順序遞增排列

#### Scenario: 依建立日期排序

- **WHEN** body 包含 `"sortField": "createdDate", "sortOrder": "desc"`
- **THEN** 回傳結果依 `createdDate` 由新到舊排列

#### Scenario: 依 Id 排序

- **WHEN** body 包含 `"sortField": "id", "sortOrder": "desc"`
- **THEN** 回傳結果依 `id` 由大到小排列

#### Scenario: 未知排序欄位 fallback 為 id

- **WHEN** body 包含 `"sortField": "unknown-field"`
- **THEN** 回傳結果依 `id` 排序（不拋錯）

### Requirement: ExampleItemsView 表格欄位排序

`ExampleItemsView.vue` 表格的 `id`、`name`、`description`、`categoryName`、`createdDate` 五個欄位 SHALL 全部開啟 PrimeVue `Column` 的 `sortable`，點擊任一欄位表頭 SHALL 觸發對應 `sortField`/`sortOrder` 並重新呼叫 `POST /api/ExampleItems/Search`。

#### Scenario: 點擊類別欄位表頭排序

- **WHEN** 使用者點擊 `categoryName` 欄位表頭
- **THEN** 前端以 `sortField: "categoryName"` 呼叫 `POST /api/ExampleItems/Search` 並依回傳結果重新渲染表格

#### Scenario: 點擊建立日期欄位表頭排序

- **WHEN** 使用者點擊 `createdDate` 欄位表頭
- **THEN** 前端以 `sortField: "createdDate"` 呼叫 `POST /api/ExampleItems/Search` 並依回傳結果重新渲染表格

#### Scenario: 表格顯示建立日期欄位

- **WHEN** ExampleItemsView 表格渲染資料列
- **THEN** 每列顯示對應的 `createdDate` 欄位內容
