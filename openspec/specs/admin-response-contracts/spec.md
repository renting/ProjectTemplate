## Purpose

規範統一的 API 回應包裝格式 `ApiResponse<T>` 與分頁版 `ApiPagedResponse<T>`。

## Requirements

### Requirement: ApiResponse\<T\> 統一回傳型別
所有 Controller 端點 SHALL 使用 `ApiResponse<T>` 作為回傳型別，搭配正確的 HTTP 狀態碼，不得回傳匿名物件或裸型別。`ApiResponse<T>` 包含四個屬性：`Success`、`Message`、`Result`（單一物件）、`Results`（集合），每次回傳只有其中一個資料屬性有值。

#### Scenario: 成功回傳單一物件
- **WHEN** 操作成功且結果為單一資源（如登入、查詢單筆）
- **THEN** Controller SHALL 回傳 HTTP 2xx，body 的 `Success = true`、`Result` 為資料物件、`Results = null`

#### Scenario: 成功回傳集合
- **WHEN** 操作成功且結果為多筆資料（如列表查詢）
- **THEN** Controller SHALL 回傳 HTTP 2xx，body 的 `Success = true`、`Results` 為資料陣列、`Result = null`

#### Scenario: 錯誤回傳
- **WHEN** 操作失敗（驗證錯誤、未授權、找不到資源等）
- **THEN** Controller SHALL 回傳對應 HTTP 4xx/5xx 狀態碼，body 為 `ApiResponse<T>` 且 `Success = false`、`Message` 包含人類可讀的錯誤說明

#### Scenario: 錯誤回傳
- **WHEN** 操作失敗（驗證錯誤、未授權、找不到資源等）
- **THEN** Controller SHALL 回傳對應 HTTP 4xx/5xx 狀態碼，body 為 `ApiResponse<T>` 且 `Success = false`、`Message` 包含人類可讀的錯誤說明

#### Scenario: 禁止匿名物件
- **WHEN** 撰寫任何 Controller action
- **THEN** 不得使用 `return Ok(new { ... })` 形式，SHALL 定義具名 Response DTO

---

### Requirement: ApiResponse\<T\> 靜態工廠方法
`ApiResponse<T>` SHALL 提供 `Ok()`、`OkList()` 與 `Fail()` 靜態工廠方法，簡化 Controller 的撰寫。

#### Scenario: Ok 工廠方法（單一物件）
- **WHEN** 建立成功的單一資源 response
- **THEN** SHALL 使用 `ApiResponse<T>.Ok(result)` 或 `ApiResponse<T>.Ok(result, message)`，Result 有值，Results 為 null

#### Scenario: OkList 工廠方法（集合）
- **WHEN** 建立成功的集合 response
- **THEN** SHALL 使用 `ApiResponse<T>.OkList(results)`，Results 有值，Result 為 null

#### Scenario: Fail 工廠方法
- **WHEN** 建立失敗 response
- **THEN** SHALL 使用 `ApiResponse<object>.Fail("錯誤訊息")`，Result 與 Results 均為 null

---

### Requirement: HTTP 狀態碼與 body 一致性
HTTP 狀態碼 SHALL 正確反映操作結果，body 的 `Success` 欄位 SHALL 與 HTTP 狀態碼一致，不得出現 HTTP 200 但 `Success = false` 或 HTTP 4xx 但 `Success = true` 的情況。

#### Scenario: 狀態碼一致
- **WHEN** 回傳任何 response
- **THEN** HTTP 2xx SHALL 對應 `Success = true`，HTTP 4xx/5xx SHALL 對應 `Success = false`

#### Scenario: StatusCode 不放入 body
- **WHEN** 建構 ApiResponse 物件
- **THEN** `ApiResponse<T>` 不包含 `StatusCode` 屬性，HTTP 狀態碼由 HTTP 層傳遞，不重複放入 body

---

### Requirement: Response DTO 具名型別
每個 API 端點的 Response SHALL 定義具名 DTO class，放在對應 Feature 的 `Responses/` 資料夾。

#### Scenario: 補齊現有匿名回傳
- **WHEN** `AuthController.Me()` 目前回傳匿名物件 `new { username, displayName }`
- **THEN** SHALL 建立 `MeResponse.cs` 放在 `Features/Auth/Responses/`，並更新 action 使用 `ApiResponse<MeResponse>.Ok(...)`
