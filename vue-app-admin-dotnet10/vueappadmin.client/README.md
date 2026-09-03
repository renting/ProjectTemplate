# vueappadmin.client

Vue 3 後台管理前端，搭配 `VueAppAdmin.Server`（ASP.NET Core 8）使用。
通常不單獨啟動，請從 server 專案啟動以取得完整的 SPA proxy 環境。

## 技術棧

| 類別 | 套件 |
|---|---|
| 框架 | Vue 3 + TypeScript |
| 路由 | Vue Router 5 |
| 狀態管理 | Pinia |
| UI 元件 | PrimeVue 4（Aura theme）|
| CSS 框架 | Bootstrap 5 + Bootstrap Icons |
| 圖示 | Bootstrap Icons、Prime Icons、FontAwesome 7 |
| HTTP | Axios |
| 表單驗證 | Vee-Validate 4 + Yup |
| 建置工具 | Vite 8 |
| Linter | ESLint + Oxlint |

## 開發指令

```sh
# 安裝套件（建議使用 pnpm）
pnpm install

# 開發模式（通常由 server 專案觸發，不需手動執行）
pnpm run dev

# 型別檢查 + 正式建置
pnpm run build

# Lint 修正
pnpm run lint

# 執行測試（--run 為非 watch mode，執行完畢後自動結束，不卡住 terminal）
npm run test -- --run
```

> **注意**：`pnpm run dev` 會自動以 `dotnet dev-certs` 產生 HTTPS 憑證。
> 若憑證不存在，需確認已安裝 .NET SDK。

## 目錄結構

```
src/
├── api/                    # API 呼叫模組（每個 feature 一個檔案）
│   ├── auth.api.ts
│   ├── example-items.api.ts
│   ├── example-categories.api.ts
│   ├── menu.api.ts
│   └── features.api.ts
├── components/
│   ├── MainLayout/
│   │   ├── MainHeader.vue
│   │   └── MainSidebar.vue
│   └── NavigationProgress.vue
├── lib/
│   └── axios.ts            # 中央 axios instance（interceptors 在此）
├── router/
│   └── index.ts
├── stores/
│   ├── auth-store.ts
│   └── user-info-store.ts
├── types/
│   ├── api.ts              # ApiResponse<T> 及所有 server API 合約型別
│   └── router.d.ts         # RouteMeta 型別擴充
└── views/
    ├── layouts/
    │   └── MainLayout.vue
    ├── DashboardView.vue
    ├── ExampleItemsView.vue
    ├── LoginView.vue
    └── NotFoundView.vue
```

## 核心架構

### API 通訊層

所有 API 呼叫都透過 `src/lib/axios.ts` 的 `apiClient` instance，**不直接 import axios**。

```
view / store
  → src/api/<feature>.api.ts   ← 呼叫 apiClient，unwrap ApiResponse<T>
      → src/lib/axios.ts       ← interceptors 處理 token、錯誤、401
          → /api/...           ← Vite proxy → ASP.NET Core
```

**Request interceptor**：每次請求前從 `localStorage` 讀取 `{VITE_APP_NAME}_authToken`，自動帶入 `Authorization: Bearer` header。

**Response interceptor**：
- `success: false` → 丟出含 server `message` 的 `Error`
- HTTP 401（非 login endpoint）→ 清除 token，redirect 到 `/login`
- HTTP 401（`/api/Auth/Login`）→ 讓錯誤往上傳，由 LoginView 顯示訊息

### 新增 Feature API

1. 在 `src/types/api.ts` 加入 Request / Response 型別
2. 新增 `src/api/<feature>.api.ts`，用 `apiClient` 呼叫並 unwrap `result` / `results`
3. 在 view 或 store 中 import 函式使用，**勿直接使用 axios**

```ts
// src/api/example.api.ts
import apiClient from '@/lib/axios';
import type { ApiResponse, ExampleResponse } from '@/types/api';

export async function getExample(id: number): Promise<ExampleResponse> {
    const { data } = await apiClient.get<ApiResponse<ExampleResponse>>(`/api/Example/${id}`);
    return data.result!;
}
```

### 新增路由與 Sidebar 選單

在 `src/router/index.ts` 的 `children` 加入 route，設定 meta：

```ts
{
    path: 'my-feature',
    name: 'my-feature',
    component: () => import('@/views/MyFeatureView.vue'),
    meta: {
        showInSidebar: true,
        sidebarLabel: '我的功能',
        sidebarIcon: 'bi-star',   // Bootstrap Icons class name
        title: '我的功能'          // 顯示於 browser tab
    }
}
```

`showInSidebar: true` 的 route 會自動出現在 `MainSidebar`，不需要額外修改 sidebar 元件。

### Auth 流程

```
登入 → login() → auth-store.login(token) → localStorage
                → userInfoStore.fetchUserInfo()
                → router.push('/dashboard')

重整頁面 → axios interceptor 自動從 localStorage 讀 token（不需 init()）

Token 過期 → 任意 API 請求回 401 → interceptor 清 token → redirect /login
```

## 環境變數

| 變數 | 說明 | 預設值 |
|---|---|---|
| `DEV_SERVER_PORT` | Vite dev server 埠號 | `23288` |
| `ASPNETCORE_HTTPS_PORT` | proxy 目標的 server 埠號 | `7173` |
| `VITE_DEFINECONFIG_BASE` | 生產環境部署子路徑 | `/` |

複製 `.env.example` 為 `.env` 填入實際值（`.env` 不納入版本控制）。
