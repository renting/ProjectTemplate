import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { mount, flushPromises } from '@vue/test-utils';
import { setActivePinia, createPinia } from 'pinia';
import PrimeVue from 'primevue/config';
import ExampleItemsView from '../ExampleItemsView.vue';
import type { ExampleCategoryResponse, ItemResponse } from '@/types/api';

vi.mock('@/api/example-items.api');
vi.mock('@/api/example-categories.api');

import { searchItems } from '@/api/example-items.api';
import { getExampleCategories } from '@/api/example-categories.api';

const mockCategories: ExampleCategoryResponse[] = [
    { id: 1, name: 'Category A' },
    { id: 2, name: 'Category B' },
];

const mockItems: ItemResponse[] = [
    { id: 1, name: 'Item 1', description: 'Desc 1', categoryId: 1, categoryName: 'Category A', createdDate: '2026-06-01T00:00:00' },
];

function mountComponent() {
    return mount(ExampleItemsView, {
        global: {
            plugins: [PrimeVue],
            stubs: {
                DataTable: true,
                Column: true,
                MultiSelect: true,
                DatePicker: true,
            },
        },
    });
}

describe('ExampleItemsView', () => {
    beforeEach(() => {
        setActivePinia(createPinia());
        vi.mocked(getExampleCategories).mockResolvedValue(mockCategories);
        vi.mocked(searchItems).mockResolvedValue({ items: mockItems, total: 1 });
    });

    afterEach(() => {
        vi.useRealTimers();
        vi.clearAllMocks();
    });

    it('初始掛載完成後資料載入', async () => {
        const wrapper = mountComponent();
        await flushPromises();

        expect(wrapper.vm.items).toHaveLength(1);
        expect(wrapper.vm.loading).toBe(false);
    });

    it('API 失敗顯示錯誤提示', async () => {
        vi.mocked(searchItems).mockRejectedValue(new Error('網路錯誤'));

        const wrapper = mountComponent();
        await flushPromises();

        expect(wrapper.find('.alert-danger').exists()).toBe(true);
        expect(wrapper.find('.alert-danger').text()).toContain('網路錯誤');
    });

    it('名稱篩選觸發 API 重新呼叫', async () => {
        const wrapper = mountComponent();
        await flushPromises();

        const nameInput = wrapper.find('input[placeholder="名稱搜尋..."]');
        await nameInput.setValue('item 1');
        await wrapper.find('button.btn-primary').trigger('click');
        await flushPromises();

        expect(searchItems).toHaveBeenCalledTimes(2);
        expect(vi.mocked(searchItems).mock.calls[1][0].name).toBe('item 1');
    });

    it('分頁切換觸發 API 重新呼叫', async () => {
        const wrapper = mountComponent();
        await flushPromises();

        wrapper.vm.onPage({ first: 10, rows: 10 });
        await flushPromises();

        expect(searchItems).toHaveBeenCalledTimes(2);
        const secondCall = vi.mocked(searchItems).mock.calls[1][0];
        expect(secondCall.page).toBe(2);
        expect(secondCall.pageSize).toBe(10);
    });
});
