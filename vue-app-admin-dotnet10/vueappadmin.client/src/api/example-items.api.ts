import apiClient from '@/lib/axios';
import type { ApiPagedResponse, ApiResponse, ExampleItemsSearchRequest, ItemResponse } from '@/types/api';

// 分頁搜尋，回傳當頁資料與總筆數（供前端計算總頁數）
export async function searchItems(request: ExampleItemsSearchRequest): Promise<{ items: ItemResponse[]; total: number }> {
    const { data } = await apiClient.post<ApiPagedResponse<ItemResponse>>('/api/ExampleItems/Search', request);
    return { items: data.results ?? [], total: data.total };
}

export async function getItemById(id: number): Promise<ItemResponse> {
    const { data } = await apiClient.get<ApiResponse<ItemResponse>>(`/api/ExampleItems/${id}`);
    return data.result!;
}
