<script setup lang="ts">
import { ref, onMounted } from 'vue';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import InputText from 'primevue/inputtext';
import MultiSelect from 'primevue/multiselect';
import DatePicker from 'primevue/datepicker';
import { searchItems } from '@/api/example-items.api';
import { getExampleCategories } from '@/api/example-categories.api';
import type { ExampleCategoryResponse, ExampleItemsSearchRequest, ItemResponse } from '@/types/api';

const items = ref<ItemResponse[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);
const totalRecords = ref(0);

const categories = ref<ExampleCategoryResponse[]>([]);
const filterName = ref('');
const filterDescription = ref('');
const filterCategoryIds = ref<ExampleCategoryResponse[]>([]);
// PrimeVue DatePicker range 模式：[起始日期, 結束日期]，任一端未選為 null
const filterDateRange = ref<(Date | null)[] | null>(null);

// 轉為 yyyy-MM-dd（本地時區），避免 toISOString() 轉 UTC 造成日期偏移一天
function toDateOnly(date: Date): string {
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
}

function formatDate(value: string): string {
    return value ? value.slice(0, 10) : '';
}

// PrimeVue DataTable lazy mode 的分頁/排序狀態
// first = 目前頁第一筆的 offset（0-based），rows = 每頁筆數
const lazyParams = ref({
    first: 0,
    rows: 10,
    sortField: 'id',
    sortOrder: 'asc',
});

function buildRequest(): ExampleItemsSearchRequest {
    const { first, rows, sortField, sortOrder } = lazyParams.value;
    const [rangeStart, rangeEnd] = filterDateRange.value ?? [];
    return {
        page: Math.floor(first / rows) + 1,
        pageSize: rows,
        sortField,
        sortOrder,
        name: filterName.value || undefined,
        description: filterDescription.value || undefined,
        categoryIds: filterCategoryIds.value.map((c) => c.id),
        dateFrom: rangeStart ? toDateOnly(rangeStart) : undefined,
        dateTo: rangeEnd ? toDateOnly(rangeEnd) : undefined,
    };
}

async function loadItems() {
    loading.value = true;
    error.value = null;
    try {
        const result = await searchItems(buildRequest());
        items.value = result.items;
        totalRecords.value = result.total;
    } catch (err) {
        error.value = err instanceof Error ? err.message : '載入失敗';
    } finally {
        loading.value = false;
    }
}

function onSearch() {
    lazyParams.value.first = 0;
    loadItems();
}

function onPage(event: { first: number; rows: number }) {
    lazyParams.value.first = event.first;
    lazyParams.value.rows = event.rows;
    loadItems();
}

// PrimeVue DataTable sortOrder: 1 = asc, -1 = desc
function onSort(event: { sortField: string; sortOrder: number }) {
    lazyParams.value.first = 0;
    lazyParams.value.sortField = event.sortField;
    lazyParams.value.sortOrder = event.sortOrder === -1 ? 'desc' : 'asc';
    loadItems();
}

onMounted(async () => {
    categories.value = await getExampleCategories();
    await loadItems();
});

defineExpose({ items, loading, error, categories, onPage, onSort, onSearch, formatDate });
</script>

<template>
    <div>
        <h2 class="mb-3">Example Items</h2>

        <div class="row g-2 mb-3 align-items-center">
            <div class="col-12 col-md-2">
                <InputText
                    v-model="filterName"
                    placeholder="名稱搜尋..."
                    class="w-100"
                    @keyup.enter="onSearch"
                />
            </div>
            <div class="col-12 col-md-2">
                <InputText
                    v-model="filterDescription"
                    placeholder="說明搜尋..."
                    class="w-100"
                    @keyup.enter="onSearch"
                />
            </div>
            <div class="col-12 col-md-3">
                <MultiSelect
                    v-model="filterCategoryIds"
                    :options="categories"
                    optionLabel="name"
                    placeholder="選擇類別..."
                    class="w-100"
                />
            </div>
            <div class="col-12 col-md-3">
                <DatePicker
                    v-model="filterDateRange"
                    selectionMode="range"
                    dateFormat="yy-mm-dd"
                    placeholder="建立日期區間..."
                    showIcon
                    class="w-100"
                />
            </div>
            <div class="col-12 col-md-2">
                <button class="btn btn-primary w-100" @click="onSearch">查詢</button>
            </div>
        </div>

        <div v-if="error" class="alert alert-danger d-flex align-items-center gap-2">
            <i class="bi bi-exclamation-triangle-fill"></i>
            <span>{{ error }}</span>
            <button class="btn btn-sm btn-outline-danger ms-auto" @click="loadItems">重試</button>
        </div>
        <DataTable
            v-else
            :value="items"
            :loading="loading"
            :lazy="true"
            :paginator="true"
            :rows="lazyParams.rows"
            :totalRecords="totalRecords"
            @page="onPage"
            @sort="onSort"
        >
            <Column field="id" header="ID" style="width: 80px" :sortable="true" />
            <Column field="name" header="名稱" :sortable="true" />
            <Column field="description" header="說明" :sortable="true" />
            <Column field="categoryName" header="類別" style="width: 120px" :sortable="true" />
            <Column field="createdDate" header="建立日期" style="width: 140px" :sortable="true">
                <template #body="{ data }">{{ formatDate(data.createdDate) }}</template>
            </Column>
        </DataTable>
    </div>
</template>
