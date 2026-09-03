## Purpose

規範前端元件測試的撰寫方式與涵蓋範圍。

## Requirements

### Requirement: ExampleItemsView 元件測試

`src/views/__tests__/ExampleItemsView.spec.ts` SHALL 存在，使用 Vitest + `@vue/test-utils` 測試 `ExampleItemsView.vue` 的核心行為。

測試設定 SHALL：
- 以 `vi.mock('@/api/example-items.api')` 與 `vi.mock('@/api/example-categories.api')` mock API 模組
- 以 `global.stubs` 替換 `DataTable`、`Column`、`MultiSelect` 為空 stub
- 每個 test case 使用 `await flushPromises()` 等待 `onMounted` 非同步完成

#### Scenario: 初始掛載完成後資料載入

- **WHEN** 元件掛載，`searchItems` mock 回傳 `{ items: [{ id: 1, name: 'Item 1', ... }], total: 1 }`
- **THEN** `wrapper.vm.items` SHALL 等於 mock 回傳的陣列，`wrapper.vm.loading` SHALL 為 `false`

#### Scenario: API 失敗顯示錯誤提示

- **WHEN** `searchItems` mock 拋出 `new Error('網路錯誤')`，元件掛載後 `flushPromises()` 完成
- **THEN** DOM 中 SHALL 出現 `.alert-danger` 元素，且其文字內容包含 `'網路錯誤'`

#### Scenario: 名稱篩選觸發 API 重新呼叫

- **WHEN** 初始掛載完成後，對名稱 `input` 設值並觸發 `input` 事件，等待 debounce（使用 `vi.useFakeTimers()` 前進 300ms）後呼叫 `flushPromises()`
- **THEN** `searchItems` SHALL 被呼叫第二次，且呼叫參數的 `name` 欄位等於輸入值

#### Scenario: 分頁切換觸發 API 重新呼叫

- **WHEN** 初始掛載完成後，直接呼叫 `wrapper.vm.onPage({ first: 10, rows: 10 })`，再 `flushPromises()`
- **THEN** `searchItems` SHALL 被呼叫，且呼叫參數的 `page` 為 `2`、`pageSize` 為 `10`
