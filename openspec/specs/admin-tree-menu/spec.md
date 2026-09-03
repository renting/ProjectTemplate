## Purpose

規範選單樹的資料結構、依使用者功能權限過濾的行為與前端呈現方式。

## Requirements

### Requirement: MenuNode 樹狀資料結構

系統 SHALL 以遞迴 `MenuNode` 結構定義選單，每個節點包含：
- `id: int`
- `label: string`
- `icon: string`（Bootstrap Icons class）
- `route: string | null`（null 表示群組節點，不可直接導航）
- `requiredFeature: string | null`（null 表示無需特定 feature）
- `children: MenuNode[]`

Hardcode 選單結構（未過濾的完整樹）：
- 儀表板（route: `/dashboard`，requiredFeature: null）
- 資料管理（route: null，requiredFeature: null）
  - 範例清單（route: `/example-items`，requiredFeature: `items:read`）
  - 類別管理（route: `/example-categories`，requiredFeature: `categories:manage`）
- 系統管理（route: null，requiredFeature: null）
  - 群組管理（route: `/groups`，requiredFeature: `menu:admin`）

#### Scenario: 完整樹結構正確定義

- **WHEN** 系統初始化 MenuService
- **THEN** 完整樹包含 3 個頂層節點，其中 2 個有子節點共 3 個葉節點

---

### Requirement: POST /api/Menu/Items 後端過濾

系統 SHALL 提供 `POST /api/Menu/Items`（需 JWT），從 JWT claims 讀取使用者 features，遞迴過濾選單節點後回傳。過濾規則：

1. 節點的 `requiredFeature` 為 null → 保留
2. 節點的 `requiredFeature` 在使用者 features 中 → 保留
3. 節點的 `requiredFeature` 不在使用者 features 中 → 移除（含其所有子節點）
4. 群組節點（route: null）過濾後若所有子節點均被移除 → 群組節點本身也移除

#### Scenario: admin 取得完整選單

- **WHEN** `admin` 呼叫 `POST /api/Menu/Items`
- **THEN** 回傳完整 3 層樹，包含所有子節點

#### Scenario: viewer 僅看到有權限的節點

- **WHEN** `viewer` 呼叫 `POST /api/Menu/Items`
- **THEN** 回傳：儀表板、資料管理（僅含「範例清單」子節點）；「類別管理」、「系統管理」整個群組均不出現

#### Scenario: 群組節點子節點全無權限時自動隱藏

- **WHEN** 使用者不具備 `menu:admin` 且系統管理下只有「群組管理」一個子節點
- **THEN** 「系統管理」群組節點本身也從回傳結果中移除

---

### Requirement: MainSidebar.vue 遞迴渲染

前端 `MainSidebar.vue` SHALL 遞迴渲染 MenuNode 樹，群組節點支援點擊展開/收合，葉節點點擊後導航至對應 route。

#### Scenario: 群組節點可展開收合

- **WHEN** 使用者點擊群組節點（route: null）
- **THEN** 其子節點清單切換顯示/隱藏

#### Scenario: 葉節點點擊導航

- **WHEN** 使用者點擊有 route 的節點
- **THEN** Vue Router 導航至該 route
