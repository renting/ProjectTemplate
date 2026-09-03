## Purpose

規範程式碼註解的撰寫要求，使範本使用者能理解每個檔案的用途與可替換點。

## Requirements

### Requirement: 所有原始碼具備行內說明註解

所有 C# 與 TypeScript/Vue 原始碼 SHALL 具備繁體中文行內註解，說明非直覺的設計決策、重要限制或需要替換的 hardcode 值。註解 SHALL 只在有說明價值之處加入，不得對自說明的程式碼逐行翻譯。

#### Scenario: 一般邏輯檔案

- **WHEN** 開發者閱讀任意 `.cs`、`.ts`、`.vue` 檔案
- **THEN** 關鍵邏輯段落、非直覺的設計選擇有繁體中文說明，可理解設計意圖

#### Scenario: 純介面或簡單 DTO

- **WHEN** 開發者閱讀介面宣告（`IAuthService.cs` 等）或簡單 Response record
- **THEN** 可無行內說明，或只在有說明價值時加入（不強制每個屬性都加）

### Requirement: GroupFeatureStore 明確標注為 demo 轉換表

`GroupFeatureStore.cs` 的兩個 Dictionary（`_userGroups`、`_groupFeatures`）SHALL 加上顯著的 TODO 註解，說明這是範本用的 hardcode demo 資料，實際專案 MUST 替換為資料庫查詢或其他持久化機制。

#### Scenario: 開發者看到 GroupFeatureStore

- **WHEN** 開發者閱讀 `GroupFeatureStore.cs`
- **THEN** 在兩個 Dictionary 上方看到明確的 TODO 標注與說明，知道需要替換

### Requirement: template.json 排除清單確認正確

`template.json` 的 `exclude` 清單 SHALL 涵蓋所有不應複製進新專案的檔案類型，且不得遺漏或錯誤排除。

#### Scenario: 驗證排除清單完整性

- **WHEN** 逐一比對 `exclude` 清單與實際專案目錄結構
- **THEN** `pnpm-lock.yaml`、`pnpm-workspace.yaml`、`package-lock.json`、`bin/`、`obj/`、`dist/`、`node_modules/`、`logs/` 均在排除清單中
