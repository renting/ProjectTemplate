## Purpose

規範以群組為單位的功能權限模型，以及功能識別字在 token 與選單過濾中的使用方式。

## Requirements

### Requirement: Group + Feature 三層權限模型

系統 SHALL 以 User → Groups → Features 三層結構管理權限。每位使用者屬於一個或多個 Group，每個 Group 擁有一組 Feature 代碼。Feature 代碼格式為 `resource:action`（如 `items:read`、`items:write`）。所有資料 hardcode in-memory。

預設使用者：
- `admin` / `password` → Groups: `["SuperAdmins"]` → Features: `["items:read", "items:write", "categories:manage", "menu:admin"]`
- `viewer` / `password` → Groups: `["ReadOnly"]` → Features: `["items:read"]`

#### Scenario: admin 登入後取得完整 features

- **WHEN** `admin` 使用正確密碼登入並呼叫 `GET /api/Auth/Me`
- **THEN** response 包含 `groups: ["SuperAdmins"]` 與完整 features 陣列

#### Scenario: viewer 登入後僅取得受限 features

- **WHEN** `viewer` 使用正確密碼登入並呼叫 `GET /api/Auth/Me`
- **THEN** response 包含 `groups: ["ReadOnly"]` 與 `features: ["items:read"]`

---

### Requirement: Features 寫入 JWT Claims

系統 SHALL 於登入時將使用者的 features[] 序列化為逗號分隔字串寫入 JWT claim（claim key: `features`）。後端 API 可直接從 token 讀取，無需額外查詢。

#### Scenario: JWT 含有 features claim

- **WHEN** `admin` 登入成功
- **THEN** 所發出的 JWT 包含 `features` claim，值為 `"items:read,items:write,categories:manage,menu:admin"`

---

### Requirement: GET /api/Features 端點

系統 SHALL 提供 `GET /api/Features`（需 JWT），回傳所有已定義的 Feature 代碼清單及說明。

#### Scenario: 取回全部 feature 清單

- **WHEN** 已登入使用者呼叫 `GET /api/Features`
- **THEN** response 包含系統中所有 feature 代碼，不論呼叫者本身擁有哪些 features

---

### Requirement: 前端 hasFeature() helper

`user-info-store` SHALL 提供 `hasFeature(feature: string): boolean` 方法，讓 template 可用 `v-if="userInfoStore.hasFeature('items:write')"` 控制 UI 顯示。

#### Scenario: 有權限時回傳 true

- **WHEN** `admin` 已登入，呼叫 `hasFeature('items:write')`
- **THEN** 回傳 `true`

#### Scenario: 無權限時回傳 false

- **WHEN** `viewer` 已登入，呼叫 `hasFeature('items:write')`
- **THEN** 回傳 `false`
