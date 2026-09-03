## Purpose

規範前端的登入流程、token 保存、路由守衛與 401 自動登出行為。

## Requirements

### Requirement: auth-store 管理 JWT token 狀態
`auth-store.ts` SHALL 以 Pinia 定義，state 包含 `isAuthenticated: boolean`，初始值依 `localStorage.getItem('SiteToken')` 是否存在決定。`auth-store` SHALL NOT 操作 `axios.defaults.headers` 或任何 axios 狀態；token 注入責任完全由 `src/lib/axios.ts` 的 request interceptor 承擔。`init()` action SHALL NOT 存在，不需要在應用程式啟動時呼叫任何初始化方法。

#### Scenario: 頁面重整後維持登入狀態
- **WHEN** 使用者重整頁面，`localStorage` 存有 `SiteToken`
- **THEN** `isAuthenticated` 為 `true`；後續 API 請求由 interceptor 自動攜帶 Bearer token，無需 store 介入

#### Scenario: 登入後更新狀態
- **WHEN** `login(token)` action 被呼叫
- **THEN** token 存入 `localStorage`，`isAuthenticated` 設為 `true`；不操作 axios headers

#### Scenario: 登出後清除狀態
- **WHEN** `logout()` action 被呼叫
- **THEN** `localStorage` 的 `SiteToken` 被移除，`isAuthenticated` 設為 `false`；不操作 axios headers

### Requirement: LoginView 提供帳號密碼登入表單
`LoginView.vue` SHALL 使用 Vee-Validate + Yup 做表單驗證，欄位包含 `username`（必填）與 `password`（必填），表單 input SHALL 設定對應的 `autocomplete` 屬性（`username`、`current-password`）。送出後呼叫 `src/api/auth.api.ts` 的 `login()` function（不直接使用 axios），成功後導向 `/dashboard`，失敗顯示 server 回傳的 `error.message`。表單提交期間 SHALL 以 `isSubmitting` ref 控制按鈕 disabled 狀態與 spinner 顯示，防止重複提交。

#### Scenario: 登入成功導向 dashboard
- **WHEN** 使用者輸入有效帳密並提交
- **THEN** `auth-store.login(token)` 被呼叫，router 導向 `/dashboard`

#### Scenario: 登入失敗顯示 server 錯誤訊息
- **WHEN** 使用者輸入無效帳密並提交
- **THEN** 表單顯示 server 回傳的 message（如「帳號或密碼錯誤」），不跳轉頁面，不觸發 redirect

#### Scenario: 提交期間按鈕 disabled 且顯示 spinner
- **WHEN** 使用者點擊登入，API 請求進行中
- **THEN** 送出按鈕顯示 loading spinner 並為 disabled，輸入欄位亦為 disabled，無法重複提交

#### Scenario: 空白欄位提交觸發驗證
- **WHEN** 使用者未填寫欄位即按送出
- **THEN** Vee-Validate 顯示欄位必填錯誤，不發送 API 請求

### Requirement: router beforeEach 守衛保護需認證的路由
`router/index.ts` 的 `beforeEach` 守衛 SHALL 檢查 `auth-store.isAuthenticated`，未登入時 SHALL 導向 `/login`。`meta.noAuthRequired: true` 的路由（如 `/login`）不受守衛限制。

#### Scenario: 未登入存取受保護頁面
- **WHEN** 未登入使用者嘗試進入 `/dashboard`
- **THEN** router 導向 `/login`

#### Scenario: 已登入存取 login 頁面
- **WHEN** 已登入使用者進入 `/login`
- **THEN** router 導向 `/dashboard`（避免重複登入）

### Requirement: user-info-store 儲存登入使用者基本資訊
`user-info-store.ts` SHALL 以 Pinia 定義，state 包含 `username: string`、`displayName: string`、`isLoading: boolean`、`error: string | null`。`fetchUserInfo()` action SHALL 在執行期間設定 `isLoading = true`、`error = null`，成功後填入 `username` 與 `displayName`，失敗後設定 `error` 為錯誤訊息，finally 設定 `isLoading = false`。

#### Scenario: 登入後取得使用者資訊
- **WHEN** 登入成功並呼叫 `fetchUserInfo()` action
- **THEN** `username` 與 `displayName` 以 API 回應更新，`isLoading` 回到 `false`，`error` 為 `null`

#### Scenario: fetchUserInfo 失敗時設定 error state
- **WHEN** `GET /api/Auth/Me` 失敗
- **THEN** `error` 被設為錯誤訊息，`isLoading` 回到 `false`，`username`、`displayName` 維持原值
