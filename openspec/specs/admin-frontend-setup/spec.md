## Purpose

規範前端工具鏈組成：Vue 3、Vite、TypeScript、Pinia、Vue Router、PrimeVue、Bootstrap 與 lint 設定。

## Requirements

### Requirement: vite.config.ts 支援 HTTPS、proxy 與 @ alias
`vite.config.ts` SHALL 設定：（1）自動產生或讀取 ASP.NET Core dev cert（`.pem`/`.key`）供 HTTPS；（2）`/api` proxy 至後端（port 由 `ASPNETCORE_HTTPS_PORT` env 決定）；（3）`@` alias 指向 `src/`；（4）以 `dotenv.config()` 載入 `.env`（在 `defineConfig` 之前呼叫）。

#### Scenario: 開發期 HTTPS 正確啟動
- **WHEN** 執行 `npm run dev`
- **THEN** Vite dev server 以 HTTPS 啟動，憑證來自 ASP.NET Core dev cert 路徑

#### Scenario: API 請求被 proxy 至後端
- **WHEN** 前端發送 `GET /api/Auth/Me`
- **THEN** Vite proxy 將請求轉發至後端 `https://localhost:<port>/api/Auth/Me`

#### Scenario: @ alias 可正常 import
- **WHEN** 程式碼使用 `import { useAuthStore } from '@/stores/auth-store'`
- **THEN** TypeScript 與 Vite 均可正確解析路徑

### Requirement: 環境變數以 .env 與 .env.example 管理
`.env.example` SHALL 列出所有 `VITE_` 開頭的環境變數及說明，`.env` 為實際值（不納入版本控制）。範本 SHALL 包含 `VITE_DEFINECONFIG_BASE`（生產部署子路徑，預設 `/`）。

#### Scenario: .env.example 存在且含必要變數
- **WHEN** 開發者 clone 專案後查看 `.env.example`
- **THEN** 可得知所有必要環境變數的名稱與用途說明

### Requirement: 前端套件清單包含完整管理系統所需依賴
`package.json` dependencies SHALL 包含：`pinia`、`vue-router`（^5.x）、`axios`、`primevue`、`@primeuix/themes`（取代已 deprecated 的 `@primevue/themes`）、`primeicons`、`bootstrap`、`bootstrap-icons`、`@fortawesome/fontawesome-svg-core`、`@fortawesome/free-solid-svg-icons`、`@fortawesome/free-regular-svg-icons`、`@fortawesome/free-brands-svg-icons`、`@fortawesome/vue-fontawesome`、`vee-validate`、`@vee-validate/yup`、`yup`、`date-fns`、`lodash-es`、`uuid`。devDependencies SHALL 包含 `dotenv`（^17.x）與 `npm-run-all2`（^9.x）。

#### Scenario: pnpm install 成功無衝突
- **WHEN** 執行 `pnpm install`
- **THEN** 所有套件安裝成功，無 peer dependency 衝突，且 `@primevue/themes` 不存在於 node_modules 中

#### Scenario: @primevue/themes 已移除
- **WHEN** 查看 `package.json` dependencies
- **THEN** 不存在 `@primevue/themes` 條目，僅存在 `@primeuix/themes`

### Requirement: main.ts 完整初始化所有套件
`main.ts` SHALL 初始化 Vue app 並 `use()` PrimeVue（含 theme）、Pinia、Vue Router。`main.ts` SHALL NOT 呼叫 `auth-store.init()`，亦不 import `useAuthStore`；token 注入由 `src/lib/axios.ts` 的 request interceptor 在每次請求時自動處理，不需要啟動時的初始化步驟。PrimeVue theme preset SHALL 從 `@primeuix/themes/aura` 匯入。

#### Scenario: 應用程式啟動完整初始化
- **WHEN** 前端應用程式啟動
- **THEN** PrimeVue、Pinia、Vue Router 均已掛載；不存在 `authStore.init()` 呼叫

#### Scenario: 頁面重整後 API 請求仍自動攜帶 token
- **WHEN** 使用者重整頁面後（`main.ts` 重新執行），`localStorage` 存有 `SiteToken`，並發出 API 請求
- **THEN** 請求自動攜帶 Authorization header，無需 `main.ts` 做任何 token 相關初始化

### Requirement: main.css 不含 scaffold 殘留樣式
`main.css` SHALL 只包含 `@import './base.css'`，不得包含 `#app` 的 max-width / padding 限制、`.green` class、`a` 的 scaffold 連結樣式、或任何 `@media` 將 `body` 設為 `display: flex` 的規則。

#### Scenario: 桌面寬度下 body 不被 flexbox 置中
- **WHEN** 瀏覽器視窗寬度 ≥ 1024px
- **THEN** `body` 不帶 `display: flex`，`MainLayout` 的 `vh-100 d-flex flex-column` 正常填滿視窗高度

#### Scenario: #app 不限制最大寬度
- **WHEN** `main.css` 被載入
- **THEN** `#app` 元素無 `max-width` 與 `padding` 限制，admin layout 可填滿全部可用空間

### Requirement: base.css 只保留 body 基礎樣式
`base.css` SHALL 包含 box-sizing reset（`*, *::before, *::after { box-sizing: border-box }`）與 `body` 的 `min-height`、`font-family`、`font-size`、`line-height` 設定，不得包含 `--vt-c-*` CSS 自訂屬性、`--section-gap`、semantic color 變數、或 `@media (prefers-color-scheme: dark)` block。

#### Scenario: 無 --vt-c-* 變數宣告
- **WHEN** 開發者在元件中嘗試使用 `var(--vt-c-white)`
- **THEN** 變數無值（undefined），不存在於 CSS 中

#### Scenario: body 基礎樣式正常套用
- **WHEN** 應用程式載入
- **THEN** `body` 套用正確的 `font-family`（`Inter, -apple-system, ...`）與 `font-size: 15px`
