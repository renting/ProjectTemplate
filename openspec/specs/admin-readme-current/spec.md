## Purpose

規範範本內各 README 必須與現況一致的項目（測試指令、API 端點清單、測試檔案結構）。

## Requirements

### Requirement: vueappadmin.client README 使用非阻塞式測試指令

`vueappadmin.client/README.md` 的前端測試執行指令 SHALL 為 `npm run test -- --run`，以非 watch mode 執行 Vitest，避免卡住 terminal。

#### Scenario: 開發者執行前端測試

- **WHEN** 開發者依照 README 執行前端測試
- **THEN** 指令為 `npm run test -- --run`，測試執行完畢後自動結束，不進入 watch mode

### Requirement: VueAppAdmin.Server README 涵蓋所有 feature 的 API endpoint

`VueAppAdmin.Server/README.md` 的 API Endpoint 文件 SHALL 包含目前所有已實作的 feature：Auth、ExampleItems、ExampleCategories、FeatureList、Menu。

#### Scenario: 開發者查詢 API 清單

- **WHEN** 開發者閱讀 `VueAppAdmin.Server/README.md`
- **THEN** 能找到 Menu、FeatureList、ExampleCategories 的 endpoint 說明（不只有 Auth 和 ExampleItems）

### Requirement: VueAppAdmin.Server.Tests README 反映現有測試檔案

`VueAppAdmin.Server.Tests/README.md` 的目錄結構說明 SHALL 包含所有現有測試類別：AuthServiceTests、ExampleItemsServiceTests、ExampleCategoriesServiceTests、MenuServiceTests。

#### Scenario: 開發者查閱測試結構

- **WHEN** 開發者閱讀 `VueAppAdmin.Server.Tests/README.md`
- **THEN** 目錄結構與命名慣例說明涵蓋 ExampleCategories 與 Menu 測試，不僅列出 Auth
