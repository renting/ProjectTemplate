## Purpose

規範 admin layout 的版面組成：Header、Sidebar、響應式 drawer、dark/light mode 切換。

## Requirements

### Requirement: NavigationProgress 顯示路由切換進度
`src/components/NavigationProgress.vue` SHALL 在 `App.vue` 最頂層掛載，使用 PrimeVue `ProgressBar`（`import ProgressBar from 'primevue/progressbar'`）實作頂部進度條。元件 SHALL 於 `router.beforeEach` 時顯示（width 從 30% 開始，200ms 後推進至 70%），於 `router.afterEach` 時完成至 100% 後淡出隱藏。進度條 SHALL 固定在頁面頂部（`position: fixed; top: 0`），高度 3px，z-index 9999。

#### Scenario: 路由切換時顯示進度條
- **WHEN** 使用者點擊 sidebar 連結觸發路由切換
- **THEN** 頁面頂部出現 3px 進度條，切換完成後淡出消失

#### Scenario: 快速切換路由進度條正確結束
- **WHEN** 路由切換完成（router.afterEach 觸發）
- **THEN** 進度條推進至 100% 並在 300ms 後隱藏，不殘留在畫面上

### Requirement: MainLayout 提供後台主版面骨架
`MainLayout.vue` SHALL 作為已登入頁面的 layout 元件，包含 `MainHeader`、`MainSidebar` 與主內容區 `<RouterView>`，透過 Vue Router 的 nested routes 機制掛載。在螢幕寬度 ≥768px（Bootstrap `md` 斷點）時，`MainLayout.vue` SHALL 以 CSS Grid（`minmax(8rem, max-content) 1fr`）並排顯示固定側邊欄與主內容區。在螢幕寬度 <768px 時，`MainLayout.vue` SHALL 改為單欄版面，主內容區 SHALL 佔滿可用寬度，側邊欄不佔用版面空間，改由 `MainSidebar` 內的覆蓋層（Drawer）呈現。

#### Scenario: 已登入頁面使用 MainLayout
- **WHEN** 已登入使用者進入 `/dashboard`
- **THEN** 頁面渲染 MainLayout（含 header 與 sidebar），內容區顯示 DashboardView

#### Scenario: 寬螢幕維持固定雙欄版面
- **WHEN** 已登入使用者在螢幕寬度 ≥768px 檢視任一頁面
- **THEN** 版面以 CSS Grid 並排顯示固定側邊欄與主內容區，與現行行為一致

#### Scenario: 窄螢幕改為單欄版面
- **WHEN** 已登入使用者在螢幕寬度 <768px 檢視任一頁面
- **THEN** 主內容區佔滿可用寬度，側邊欄不佔用版面空間

### Requirement: MainSidebar 從 router meta 衍生選單
`MainSidebar.vue` SHALL 讀取 router 中 `meta.showInSidebar === true` 的路由，自動產生選單連結。選單項目 SHALL 顯示 `meta.sidebarLabel` 文字，若路由設定了 `meta.sidebarIcon`（Bootstrap Icon class name，如 `bi-speedometer2`），SHALL 在文字前渲染對應的 `<i class="bi :class="sidebarIcon">` icon。元件 SHALL 依賴 Vue Router 的全域注冊取得 `<RouterLink>`，不得在 `<script setup>` 中明確 import `RouterLink`。

#### Scenario: 路由設有 sidebarIcon 時顯示 icon
- **WHEN** router 中有路由設定 `meta: { showInSidebar: true, sidebarLabel: 'Dashboard', sidebarIcon: 'bi-speedometer2' }`
- **THEN** Sidebar 顯示該選單項目，項目前方顯示 Bootstrap Icon `bi-speedometer2`

#### Scenario: 路由未設 sidebarIcon 時僅顯示文字
- **WHEN** router 中有路由設定 `meta.showInSidebar: true` 但無 `sidebarIcon`
- **THEN** Sidebar 顯示選單項目但不渲染 icon `<i>` 元素

#### Scenario: 路由加入 meta.showInSidebar 後自動出現在選單
- **WHEN** router 中有路由設定 `meta: { showInSidebar: true, sidebarLabel: 'Example Items' }`
- **THEN** Sidebar 顯示該選單項目，點擊後導向對應路由

#### Scenario: 無 showInSidebar 的路由不出現在選單
- **WHEN** router 中有路由未設定 `meta.showInSidebar`（或設為 `false`）
- **THEN** Sidebar 不顯示該路由的選單項目

### Requirement: MainSidebar 於窄螢幕以 Drawer 呈現
在螢幕寬度 <768px 時，`MainSidebar.vue` SHALL 以 PrimeVue `Drawer`（`import Drawer from 'primevue/drawer'`）呈現選單內容，Drawer 的開合狀態 SHALL 綁定共用的側邊欄開合狀態（如 `useSidebarDrawer` composable 匯出的 `isSidebarOpen`）。Drawer 內部 SHALL 重用既有的 `menuNodes` 資料與 `SidebarMenuItem` 遞迴元件渲染選單，不得重複實作選單資料擷取或渲染邏輯。在螢幕寬度 ≥768px 時，側邊欄 SHALL 維持現行固定欄位呈現方式，不使用 Drawer 覆蓋層。

#### Scenario: 窄螢幕以 Drawer 呈現選單
- **WHEN** 已登入使用者在螢幕寬度 <768px 開啟側邊欄
- **THEN** 選單內容以覆蓋層（Drawer）形式顯示在畫面上，覆蓋主內容區而非擠壓其寬度

#### Scenario: 點擊 Drawer 背景遮罩關閉側邊欄
- **WHEN** 側邊欄 Drawer 開啟中，使用者點擊 Drawer 以外的背景遮罩區域
- **THEN** Drawer 關閉

#### Scenario: 點擊選單項目後自動關閉 Drawer
- **WHEN** 側邊欄 Drawer 開啟中，使用者點擊其中一個選單項目觸發路由切換
- **THEN** 路由完成切換後，Drawer SHALL 自動關閉

#### Scenario: 寬螢幕維持固定欄位呈現
- **WHEN** 已登入使用者在螢幕寬度 ≥768px 檢視頁面
- **THEN** 側邊欄以現行固定欄位方式顯示，不透過 Drawer 覆蓋層呈現

### Requirement: useSidebarDrawer 提供跨元件共用的側邊欄開合狀態
`src/composables/useSidebarDrawer.ts` SHALL 以模組層級狀態（比照 `useTheme.ts` 的模式）匯出側邊欄開合狀態與操作函式（至少包含表示目前是否開啟的狀態，以及開啟、關閉、切換三種操作），供 `MainHeader.vue` 與 `MainSidebar.vue` 共用同一份開合狀態，不透過元件 props/emit 傳遞。

#### Scenario: Header 與 Sidebar 共用同一份開合狀態
- **WHEN** `MainHeader.vue` 呼叫 composable 提供的切換操作
- **THEN** `MainSidebar.vue` 綁定的開合狀態同步更新，Drawer 對應開啟或關閉

### Requirement: MainHeader 顯示應用名稱、選單開關與登出按鈕
`MainHeader.vue` SHALL 顯示應用程式名稱及登出按鈕，點擊登出後 SHALL 以 `confirm()` 請使用者確認，確認後呼叫 `auth-store.logout()` 並導向 `/login`；取消則不執行任何操作。`MainHeader.vue` SHALL 在應用程式名稱左側提供選單開關按鈕（Bootstrap Icon `bi-list`），此按鈕 SHALL 僅在螢幕寬度 <768px 時顯示；點擊此按鈕 SHALL 切換側邊欄 Drawer 的開合狀態（透過共用的開合狀態管理，如 `useSidebarDrawer`）。

#### Scenario: 點擊登出並確認
- **WHEN** 已登入使用者點擊 Header 的登出按鈕並在 confirm 對話框選擇確定
- **THEN** `auth-store.logout()` 被呼叫，router 導向 `/login`

#### Scenario: 點擊登出但取消
- **WHEN** 已登入使用者點擊 Header 的登出按鈕並在 confirm 對話框選擇取消
- **THEN** 不執行任何操作，使用者維持在當前頁面

#### Scenario: 窄螢幕顯示選單開關按鈕
- **WHEN** 已登入使用者在螢幕寬度 <768px 檢視 Header
- **THEN** 應用程式名稱左側顯示選單開關按鈕（`bi-list`）

#### Scenario: 寬螢幕不顯示選單開關按鈕
- **WHEN** 已登入使用者在螢幕寬度 ≥768px 檢視 Header
- **THEN** 選單開關按鈕不顯示（側邊欄已固定可見）

#### Scenario: 點擊選單開關按鈕開啟側邊欄
- **WHEN** 使用者在螢幕寬度 <768px 點擊選單開關按鈕，且側邊欄 Drawer 目前關閉
- **THEN** 側邊欄 Drawer 開啟並以覆蓋層顯示於畫面上

### Requirement: DashboardView 提供統計卡與活動記錄骨架
`DashboardView.vue` SHALL 包含 4 個統計卡（使用者、已完成、進行中、異常）作為 placeholder，以及最近活動區塊（初始顯示「尚無活動記錄」空狀態）。統計卡 SHALL 使用 Bootstrap Icon 搭配對應顏色（primary/success/warning/danger）。

#### Scenario: Dashboard 顯示使用者名稱與統計骨架
- **WHEN** 已登入使用者進入 `/dashboard`
- **THEN** 頁面顯示含 `user-info-store.displayName` 的歡迎訊息，4 個統計卡可見，最近活動顯示空狀態

### Requirement: ExampleItemsView 展示列表頁模式含錯誤處理
`ExampleItemsView.vue` SHALL 以 `getAllItems()` 從 `src/api/example-items.api.ts` 取得資料，以 PrimeVue DataTable 顯示。元件 SHALL 管理 `loading`、`error` state；API 失敗時 SHALL 顯示含錯誤訊息的 alert 與「重試」按鈕，點擊重試按鈕 SHALL 重新呼叫 `loadItems()`。

#### Scenario: 頁面載入時取得資料
- **WHEN** 使用者進入 ExampleItems 頁面
- **THEN** 頁面顯示 loading 狀態，API 回應後 DataTable 呈現資料列

#### Scenario: API 失敗顯示錯誤訊息與重試按鈕
- **WHEN** `getAllItems()` 拋出 Error
- **THEN** 頁面顯示 alert 含錯誤訊息，以及「重試」按鈕；不顯示空的 DataTable

#### Scenario: 點擊重試重新載入
- **WHEN** 使用者點擊「重試」按鈕
- **THEN** `loadItems()` 被重新呼叫，loading state 重置，嘗試再次取得資料

### Requirement: router 支援 404 頁面與 document.title 更新
`router/index.ts` SHALL 包含 catch-all route（`path: '/:pathMatch(.*)*'`），指向 `NotFoundView.vue`，meta 設定 `noAuthRequired: true`。router SHALL 設定 `redirect: { name: 'dashboard' }` 於根路由 `/`，使直接進入 `/` 自動導向 dashboard。每個 route SHALL 設定 `meta.title`，`router.afterEach` SHALL 更新 `document.title` 為 `<title> | VueAppAdmin`，無 title 時 fallback 為 `VueAppAdmin`。

#### Scenario: 進入未定義路由顯示 404 頁面
- **WHEN** 使用者進入不存在的路徑（如 `/not-exist`）
- **THEN** 渲染 `NotFoundView.vue`，顯示 404 訊息與返回首頁連結

#### Scenario: 進入 / 自動導向 dashboard
- **WHEN** 已登入使用者進入 `/`
- **THEN** router redirect 至 `/dashboard`

#### Scenario: route 切換後 document.title 更新
- **WHEN** 使用者進入 `/dashboard`（`meta.title: 'Dashboard'`）
- **THEN** `document.title` 為 `Dashboard | VueAppAdmin`
