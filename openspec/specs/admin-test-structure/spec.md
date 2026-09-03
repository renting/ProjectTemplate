## Purpose

規範測試專案的組成、資料夾結構鏡射方式、mock 策略與必須涵蓋的 Service。

## Requirements

### Requirement: 獨立測試專案
Solution SHALL 包含獨立的測試專案 `VueAppAdmin.Server.Tests`，使用 xUnit 框架與 NSubstitute mock 套件。

#### Scenario: 測試專案建立
- **WHEN** 初始化 template
- **THEN** solution 中 SHALL 包含 `VueAppAdmin.Server.Tests` 專案，並加入 `.slnx` 中

#### Scenario: NuGet 套件
- **WHEN** 設定測試專案相依
- **THEN** `VueAppAdmin.Server.Tests.csproj` SHALL 包含 `xunit`、`xunit.runner.visualstudio`、`Microsoft.NET.Test.Sdk`、`NSubstitute` 參考

---

### Requirement: 測試專案結構 Mirror Source
測試專案的資料夾結構 SHALL 完全對應 source 專案的 `Features/` 結構，每個 Feature 對應一個測試子資料夾。

#### Scenario: Feature 測試資料夾
- **WHEN** source 中存在 `Features/Auth/` 與 `Features/ExampleItems/`
- **THEN** 測試專案 SHALL 對應建立 `Features/Auth/` 與 `Features/ExampleItems/` 資料夾

#### Scenario: 測試檔案命名
- **WHEN** 為 `AuthService` 撰寫測試
- **THEN** 測試檔案 SHALL 命名為 `AuthServiceTests.cs`，放在 `Features/Auth/` 下

---

### Requirement: Unit Test 結構規範
每個 Unit Test 方法 SHALL 使用 Arrange / Act / Assert 三段結構，方法命名使用 `方法名稱_情境_預期結果` 格式。

#### Scenario: 測試方法命名
- **WHEN** 為 `ValidateCredentials` 撰寫正向情境測試
- **THEN** 方法名稱 SHALL 為 `ValidateCredentials_ValidCredentials_ReturnsTrue` 形式

#### Scenario: Mock 建立方式
- **WHEN** 需要 mock 一個 Interface（如 `IUserRepository`）
- **THEN** SHALL 使用 `Substitute.For<IUserRepository>()`，不得使用 Moq 或手動實作 fake class

---

### Requirement: Service Unit Test 隔離 Repository
Service 的 Unit Test SHALL mock 所有 Repository 相依，不得連接真實資料庫或 in-memory 資料結構。

#### Scenario: AuthService 測試隔離
- **WHEN** 測試 `AuthService` 的業務邏輯
- **THEN** `IUserRepository` SHALL 以 NSubstitute mock 替換，測試結果不依賴外部狀態

---

### Requirement: 後端 Service 層單元測試

`VueAppAdmin.Server.Tests` SHALL 包含涵蓋下列 Service 的 xUnit 測試，使用 xUnit 內建 Assert（不依賴商業授權套件），視需要以 NSubstitute mock 相依介面：

- `ExampleItemsServiceTests`：分頁、排序、名稱模糊查詢、說明模糊查詢、類別複選過濾
- `ExampleCategoriesServiceTests`：回傳 3 筆資料
- `MenuServiceTests`：admin 取完整樹、viewer 過濾後結果正確、群組節點子全空時自動移除
- `AuthServiceTests`：正確密碼驗證成功、錯誤密碼驗證失敗

#### Scenario: ExampleItemsService 名稱模糊查詢測試

- **WHEN** 呼叫 `Search` 傳入 `name: "item 1"`
- **THEN** 回傳結果的每筆 Name 均包含 "item 1"（不分大小寫）

#### Scenario: MenuService 過濾測試

- **WHEN** 傳入 `features: ["items:read"]` 呼叫 `GetFilteredMenu`
- **THEN** 回傳樹中不含 `requiredFeature` 為 `categories:manage` 或 `menu:admin` 的節點，且「系統管理」群組節點不出現

#### Scenario: AuthService 驗證測試

- **WHEN** 傳入正確帳密
- **THEN** 回傳使用者資訊；傳入錯誤密碼時回傳 null 或拋出例外

---

### Requirement: 前端單元測試基礎架構

前端 SHALL 安裝 `vitest`、`@vue/test-utils`、`happy-dom`、`@vitest/coverage-v8`，並在 `vite.config.ts` 加入 test 設定。

#### Scenario: 測試指令可執行

- **WHEN** 執行 `npm run test`
- **THEN** Vitest 執行所有 `*.spec.ts` 測試並輸出結果

---

### Requirement: 前端 composable 與 store 單元測試

前端 SHALL 包含下列測試檔：

- `useTheme.spec.ts`：init 讀取 localStorage、toggle 切換 isDark 並寫入 localStorage
- `auth-store.spec.ts`：login 儲存 token、logout 清除 token、isAuthenticated 狀態正確
- `user-info-store.spec.ts`：`hasFeature()` 有/無 feature 時回傳正確布林值

#### Scenario: useTheme toggle 測試

- **WHEN** 呼叫 `toggle()` 兩次
- **THEN** `isDark` 回到初始值，localStorage 中的 `theme` 值也對應正確

#### Scenario: auth-store login/logout 測試

- **WHEN** 呼叫 `login('fake-token')` 後呼叫 `logout()`
- **THEN** `isAuthenticated` 先為 true 後為 false，localStorage token 對應清除

#### Scenario: hasFeature 測試

- **WHEN** store 中 `features` 為 `["items:read"]`
- **THEN** `hasFeature('items:read')` 回傳 true，`hasFeature('items:write')` 回傳 false

---

### Requirement: 前端 Vue 元件測試

前端 SHALL 包含 `src/views/__tests__/ExampleItemsView.spec.ts`，使用 `@vue/test-utils` 測試 Vue 元件層行為，涵蓋 API mock、非同步載入、使用者互動觸發 API 重呼叫等 scenario。

詳細規格見 `specs/frontend-component-test/spec.md`。

#### Scenario: 元件測試檔存在且可執行

- **WHEN** 執行 `npm run test`
- **THEN** `ExampleItemsView.spec.ts` 中的所有 test case SHALL 通過，無 skipped
