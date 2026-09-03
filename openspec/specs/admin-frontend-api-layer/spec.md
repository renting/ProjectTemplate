## Purpose

規範前端的集中式 axios instance 與 feature-level API modules，以及與 server 合約型別的對應。

## Requirements

### Requirement: 集中的 axios instance 處理 token 注入與錯誤
`src/lib/axios.ts` SHALL 以 `axios.create()` 建立獨立 instance（`apiClient`），並掛載 request interceptor 與 response interceptor。Request interceptor SHALL 在每次請求前從 `localStorage.getItem('SiteToken')` 讀取 token，若存在則設定 `config.headers.Authorization = 'Bearer <token>'`。

#### Scenario: 有 token 時請求自動攜帶 Authorization header
- **WHEN** `localStorage` 存有 `SiteToken`，且透過 `apiClient` 發送任意請求
- **THEN** 請求 header 含 `Authorization: Bearer <token>`

#### Scenario: 無 token 時請求不帶 Authorization header
- **WHEN** `localStorage` 無 `SiteToken`，且透過 `apiClient` 發送請求
- **THEN** 請求不含 `Authorization` header

### Requirement: Response interceptor 統一處理 ApiResponse 錯誤與 401
Response interceptor 的 success handler SHALL 檢查 `response.data.success === false`，若成立則 `Promise.reject(new Error(response.data.message))`。Error handler SHALL 在 HTTP 401 且請求 URL 不包含 `/api/Auth/Login` 時，清除 `localStorage` 的 `SiteToken` 並執行 `window.location.href = '/login'`（僅觸發一次，以 `isRedirectingToLogin` flag 防止重複）。所有 error handler 最終 SHALL reject 含 server `message` 的 `Error`（若 body 有 `message`），否則 reject 原始 error。

#### Scenario: success:false 的 200 回應拋出 Error
- **WHEN** server 回傳 HTTP 200 且 `{ success: false, message: "操作失敗" }`
- **THEN** interceptor reject `new Error("操作失敗")`，caller 的 catch 收到此 Error

#### Scenario: 非 login endpoint 的 401 觸發登出導向
- **WHEN** 已登入使用者對 `/api/ExampleItems` 的請求收到 HTTP 401
- **THEN** `SiteToken` 從 localStorage 清除，頁面導向 `/login`

#### Scenario: login endpoint 的 401 不觸發導向
- **WHEN** `POST /api/Auth/Login` 回傳 HTTP 401（帳密錯誤）
- **THEN** 不執行 redirect，error 往上傳至 caller 的 catch；caller 可取得 server 回傳的 `message`

#### Scenario: 多個並發請求同時 401 只 redirect 一次
- **WHEN** 同時有多個請求收到 401
- **THEN** `window.location.href` 只被設定一次（`isRedirectingToLogin` flag 阻止後續觸發）

### Requirement: TS 型別對應 server ApiResponse<T>
`src/types/api.ts` SHALL 定義 `ApiResponse<T>` interface，欄位為 `success: boolean`、`message: string | null`、`result: T | null`、`results: T[] | null`，對應 server `VueAppAdmin.Server.Shared.ApiResponse<T>` 的 camelCase JSON 序列化結果。同檔 SHALL 定義 `LoginRequest`、`LoginResponse`、`MeResponse`、`ItemResponse` 型別。

#### Scenario: LoginRequest 型別可直接作為 POST body
- **WHEN** 呼叫 `apiClient.post<ApiResponse<LoginResponse>>('/api/Auth/Login', loginRequest)`
- **THEN** TypeScript 不報型別錯誤，`loginRequest` 含 `username: string` 與 `password: string`

#### Scenario: ApiResponse<T> 型別覆蓋所有回應欄位
- **WHEN** 存取 `data.result`、`data.results`、`data.success`、`data.message`
- **THEN** TypeScript 均有正確型別推導，不需要 `any` 斷言

### Requirement: Feature-level API modules 集中 API 呼叫
`src/api/auth.api.ts` SHALL 匯出 `login(request: LoginRequest): Promise<LoginResponse>` 與 `getMe(): Promise<MeResponse>`，各自使用 `apiClient` 並 unwrap `ApiResponse.result`。`src/api/example-items.api.ts` SHALL 匯出 `getAllItems(): Promise<ItemResponse[]>`（unwrap `results ?? []`）與 `getItemById(id: number): Promise<ItemResponse>`（unwrap `result`）。View 與 store SHALL 透過 API modules 呼叫 API，不直接 import `axios`。

#### Scenario: login() 回傳 LoginResponse（已 unwrap）
- **WHEN** 呼叫 `await login({ username, password })`
- **THEN** 回傳值為 `{ token: string }`，呼叫方不需要存取 `.result`

#### Scenario: getAllItems() 回傳空陣列而非 null
- **WHEN** server 回傳 `results: null` 或空陣列
- **THEN** `getAllItems()` 回傳 `[]`，呼叫方不需要做 null 檢查
