## Purpose

規範示範用的類別清單 API 端點與回應格式。

## Requirements

### Requirement: ExampleCategoryResponse 資料結構

系統 SHALL 定義 `ExampleCategoryResponse`，包含 `id: int` 與 `name: string`。Hardcode 3 筆 in-memory 資料：
- `{ id: 1, name: "A 類" }`
- `{ id: 2, name: "B 類" }`
- `{ id: 3, name: "C 類" }`

#### Scenario: 資料結構正確

- **WHEN** `ExampleCategoriesService` 初始化
- **THEN** 包含恰好 3 筆資料，id 為 1、2、3

---

### Requirement: POST /api/ExampleCategories

系統 SHALL 提供 `POST /api/ExampleCategories`（需 JWT），回傳 `ApiResponse<List<ExampleCategoryResponse>>`，包含全部 3 筆類別資料。Request body 為空（保留 POST 慣例，body 可忽略）。

#### Scenario: 回傳全部類別

- **WHEN** 已登入使用者呼叫 `POST /api/ExampleCategories`
- **THEN** response 包含 3 筆資料，success 為 true

#### Scenario: 未登入時拒絕存取

- **WHEN** 未帶 JWT 呼叫 `POST /api/ExampleCategories`
- **THEN** 回傳 HTTP 401
